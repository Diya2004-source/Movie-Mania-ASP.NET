using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MovieMania.Controllers.Guest
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Guest/Auth/Register.cshtml");
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string registrationJson = JsonSerializer.Serialize(model);
                TempData["RegistrationData"] = registrationJson;
                HttpContext.Session.SetString("RegData", registrationJson);

                string reference = "PAY" + DateTime.Now.Ticks.ToString().Substring(0, 8);
                HttpContext.Session.SetString("PaymentRef", reference);

                return RedirectToAction("PaymentQRCode");
            }
            return View("~/Views/Guest/Auth/Register.cshtml", model);
        }

        [HttpGet]
        public IActionResult PaymentQRCode()
        {
            string? regJson = TempData["RegistrationData"]?.ToString() ?? HttpContext.Session.GetString("RegData");

            if (string.IsNullOrEmpty(regJson))
            {
                TempData["Error"] = "Session expired. Please register again.";
                return RedirectToAction("Register");
            }

            var model = JsonSerializer.Deserialize<RegisterViewModel>(regJson);
            if (model == null)
            {
                TempData["Error"] = "Invalid registration data.";
                return RedirectToAction("Register");
            }

            string reference = HttpContext.Session.GetString("PaymentRef") ??
                              "PAY" + DateTime.Now.Ticks.ToString().Substring(0, 8);

            HttpContext.Session.SetString("PaymentRef", reference);

            ViewBag.Name = model.Name;
            ViewBag.Email = model.Email;
            ViewBag.Reference = reference;

            return View("~/Views/Guest/Auth/PaymentQRCode.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPayment()
        {
            await Task.Delay(2000);

            string? regJson = HttpContext.Session.GetString("RegData");
            string? paymentRef = HttpContext.Session.GetString("PaymentRef");

            if (!string.IsNullOrEmpty(regJson) && !string.IsNullOrEmpty(paymentRef))
            {
                var model = JsonSerializer.Deserialize<RegisterViewModel>(regJson);

                if (model != null)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == model.Email);

                    if (existingUser == null)
                    {
                        var user = new AppUser
                        {
                            Name = model.Name,
                            Email = model.Email,
                            Password = HashPassword(model.Password),
                            Role = "user",
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            ProfilePictureUrl = null,
                            ReferralCode = GenerateReferralCode(),
                            TotalReferrals = 0,
                            RewardPoints = 0
                        };

                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();

                        HttpContext.Session.Remove("RegData");
                        HttpContext.Session.Remove("PaymentRef");

                        TempData["Success"] = "Payment successful! Your account has been created. Please login.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["Error"] = "User already exists. Please login.";
                        return RedirectToAction("Login");
                    }
                }
            }

            TempData["Error"] = "Payment verification failed. Please try again.";
            return RedirectToAction("PaymentQRCode");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Guest/Auth/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);

                if (user != null && VerifyPassword(model.Password, user.Password))
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    user.LastLoginAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // ✅ DIRECT REDIRECT to User Dashboard
                    return Redirect("/User/Home/Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }
            }

            return View("~/Views/Guest/Auth/Login.cshtml", model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        // ==================== HELPER METHODS ====================

        private async Task ProcessReferralCode(string referralCode, int newUserId)
        {
            try
            {
                var referrer = await _context.Users
                    .FirstOrDefaultAsync(u => u.ReferralCode == referralCode && u.IsActive);

                if (referrer != null)
                {
                    var newUser = await _context.Users.FindAsync(newUserId);

                    var existingReferral = await _context.Referrals
                        .FirstOrDefaultAsync(r => r.ReferralCode == referralCode &&
                                                  r.ReferredUserId == newUserId);

                    if (existingReferral == null && newUser != null)
                    {
                        var referral = new Referral
                        {
                            ReferrerId = referrer.Id,
                            ReferredUserId = newUserId,
                            ReferredUserEmail = newUser.Email,
                            ReferralCode = referralCode,
                            Status = "Completed",
                            RewardAmount = 5.00m,
                            CompletedAt = DateTime.Now,
                            CreatedAt = DateTime.Now
                        };

                        _context.Referrals.Add(referral);

                        referrer.TotalReferrals += 1;
                        referrer.RewardPoints += 50;

                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing referral: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hash;
        }

        private string GenerateReferralCode()
        {
            return "REF" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
}