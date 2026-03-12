// Controllers/User/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: User/Profile
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            var user = await _context.Users
                .Include(u => u.Subscriptions)
                    .ThenInclude(s => s.SubscriptionPlan)
                .Include(u => u.Payments)
                .Include(u => u.Wishlists)
                .Include(u => u.Activities)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                ProfilePictureUrl = user.ProfilePictureUrl ?? "",

                CurrentSubscription = user.Subscriptions
                    .FirstOrDefault(s => s.Status == "Active"),
                SubscriptionHistory = user.Subscriptions
                    .OrderByDescending(s => s.StartDate)
                    .Take(5)
                    .ToList(),

                RecentPayments = user.Payments
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .ToList(),

                TotalWishlistItems = user.Wishlists?.Count(w => w.IsActive) ?? 0,
                TotalMoviesWatched = user.Activities?
                    .Count(a => a.MovieId != null && a.IsCompleted) ?? 0,
                TotalEpisodesWatched = user.Activities?
                    .Count(a => a.EpisodeId != null && a.IsCompleted) ?? 0,
                WatchTimeMinutes = (user.Activities?.Sum(a => a.WatchDuration ?? 0) ?? 0) / 60,

                RecentlyWatched = await _context.UserActivities
                    .Include(ua => ua.Movie)
                    .Include(ua => ua.Episode)
                        .ThenInclude(e => e.Show)
                    .Where(ua => ua.UserId == userId && ua.IsCompleted)
                    .OrderByDescending(ua => ua.ActivityDate)
                    .Take(10)
                    .ToListAsync(),

                WishlistPreview = await _context.Wishlists
                    .Include(w => w.Movie)
                    .Include(w => w.Show)
                    .Where(w => w.UserId == userId && w.IsActive && !w.IsWatched)
                    .OrderByDescending(w => w.Priority)
                    .Take(6)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: User/Profile/Update
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (user.Email != model.Email)
            {
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == model.Email && u.Id != userId);
                if (emailExists)
                {
                    return Json(new { success = false, message = "Email already in use" });
                }
            }

            user.Name = model.Name;
            user.Email = model.Email;

            await _context.SaveChangesAsync();
            await UpdateUserClaims(user);

            return Json(new { success = true, message = "Profile updated successfully" });
        }

        // POST: User/Profile/ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (user.Password != HashPassword(model.CurrentPassword))
            {
                return Json(new { success = false, message = "Current password is incorrect" });
            }

            user.Password = HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Password changed successfully" });
        }

        // GET: User/Profile/Referral
        public async Task<IActionResult> Referral()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(user.ReferralCode))
            {
                user.ReferralCode = GenerateReferralCode();
                await _context.SaveChangesAsync();
            }

            var referrals = await _context.Referrals
                .Where(r => r.ReferrerId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(20)
                .ToListAsync();

            var viewModel = new ReferralViewModel
            {
                ReferralCode = user.ReferralCode ?? "",
                TotalReferrals = user.TotalReferrals,
                RewardPoints = user.RewardPoints,
                ReferralHistory = referrals,
                PendingReferrals = referrals.Count(r => r.Status == "Pending"),
                CompletedReferrals = referrals.Count(r => r.Status == "Completed"),
                TotalEarned = referrals
                    .Where(r => r.Status == "Completed")
                    .Sum(r => r.RewardAmount)
            };

            return View(viewModel);
        }

        // POST: User/Profile/SendReferral
        [HttpPost]
        public async Task<IActionResult> SendReferral(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email is required" });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var existing = await _context.Referrals
                .FirstOrDefaultAsync(r => r.ReferrerId == userId && r.ReferredUserEmail == email);

            if (existing != null)
            {
                return Json(new { success = false, message = "Invitation already sent to this email" });
            }

            var referral = new Referral
            {
                ReferrerId = userId,
                ReferredUserEmail = email,
                ReferralCode = user.ReferralCode ?? GenerateReferralCode(),
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Invitation sent successfully!" });
        }

        // Helper methods
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateReferralCode()
        {
            return "REF" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        private async Task UpdateUserClaims(AppUser user)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var nameClaim = identity.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                    identity.RemoveClaim(nameClaim);

                var emailClaim = identity.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                    identity.RemoveClaim(emailClaim);

                identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity)
                );
            }
        }
    }
}