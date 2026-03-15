using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Get user's active subscription
            var activeSubscription = await _context.UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.UserId == userId && s.IsActive)
                .FirstOrDefaultAsync();

            ViewBag.ActiveSubscription = activeSubscription;

            return View("~/Views/User/Profile/Index.cshtml", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AppUser model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

            if (userId != model.Id)
                return NotFound();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Update only allowed fields
            user.Name = model.Name;

            // Check if UpdatedAt property exists before using it
            // If your AppUser model has UpdatedAt property, uncomment this:
            // user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Profile updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Verify current password
            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                return View(model);
            }

            // Update password
            user.Password = model.NewPassword;

            // Check if UpdatedAt property exists
            // if (user.GetType().GetProperty("UpdatedAt") != null)
            // {
            //     user.UpdatedAt = DateTime.Now;
            // }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Password changed successfully";

            return RedirectToAction(nameof(Index));
        }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}