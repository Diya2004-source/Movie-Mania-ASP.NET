using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System.Security.Claims;
using MovieMania.ViewModels;

namespace MovieMania.Controllers.Guest
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in as regular user, go to user dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("admin"))
                {
                    // If admin is logged in, log them out first
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                    TempData["Info"] = "Please use admin login page";
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
            }

            ViewData["ReturnUrl"] = returnUrl ?? string.Empty;
            return View("~/Views/Guest/Auth/Login.cshtml");
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Guest/Auth/Login.cshtml", model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.Password != model.Password || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                TempData["Error"] = "Invalid email or password.";
                return View("~/Views/Guest/Auth/Login.cshtml", model);
            }

            // IMPORTANT: Prevent admin login from guest login page
            if (user.Role == "admin")
            {
                ModelState.AddModelError(string.Empty, "Please use the admin login page");
                TempData["Error"] = "Admin accounts cannot login here. Please use the admin login page.";
                return View("~/Views/Guest/Auth/Login.cshtml", model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "user")
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            user.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Welcome back, {user.Name}!";

            // Check if there's a pending payment
            if (TempData["PendingPlanId"] != null)
            {
                int planId = Convert.ToInt32(TempData["PendingPlanId"]);
                TempData.Remove("PendingPlanId");
                return RedirectToAction("Process", "Payment", new { area = "User", planId = planId });
            }

            // For regular users, always go to User dashboard
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && !returnUrl.StartsWith("/Admin"))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home", new { area = "User" });
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register(string? planId = null, string? referralCode = null)
        {
            ViewBag.PlanId = planId;
            ViewBag.ReferralCode = referralCode;
            return View("~/Views/Guest/Auth/Register.cshtml");
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? planId = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PlanId = planId;
                return View("~/Views/Guest/Auth/Register.cshtml", model);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                TempData["Error"] = "Email already registered.";
                ViewBag.PlanId = planId;
                return View("~/Views/Guest/Auth/Register.cshtml", model);
            }

            var user = new AppUser
            {
                Name = model.Name ?? string.Empty,
                Email = model.Email ?? string.Empty,
                Password = model.Password ?? string.Empty,
                Role = "user", // Always set to "user" for new registrations
                IsActive = true,
                CreatedAt = DateTime.Now,
                ReferralCode = GenerateReferralCode()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // After successful registration, ALWAYS redirect to login page
            if (!string.IsNullOrEmpty(planId) && int.TryParse(planId, out int parsedPlanId))
            {
                TempData["PendingPlanId"] = parsedPlanId;
                TempData["Success"] = "Registration successful! Please login to complete your payment.";
            }
            else
            {
                TempData["Success"] = "Registration successful! Please login.";
            }

            return RedirectToAction(nameof(Login));
        }

        private string GenerateReferralCode()
        {
            return "MOVIE" + new Random().Next(1000, 9999);
        }

        // GET: /Auth/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "GuestHome");
        }

        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "GuestHome");
        }

        // GET: /Auth/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View("~/Views/Guest/Auth/AccessDenied.cshtml");
        }

        // GET: /Auth/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("~/Views/Guest/Auth/ForgotPassword.cshtml");
        }

        // POST: /Auth/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Guest/Auth/ForgotPassword.cshtml", model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null)
            {
                // Here you would send password reset email
                TempData["Success"] = "Password reset link has been sent to your email.";
            }
            else
            {
                TempData["Success"] = "If your email is registered, you will receive a password reset link.";
            }

            return RedirectToAction(nameof(Login));
        }

        // GET: /Auth/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string? token = null, string? email = null)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid password reset token.";
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Code = token
            };

            return View("~/Views/Guest/Auth/ResetPassword.cshtml", model);
        }

        // POST: /Auth/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Guest/Auth/ResetPassword.cshtml", model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                TempData["Error"] = "Invalid password reset attempt.";
                return RedirectToAction(nameof(Login));
            }

            user.Password = model.Password ?? string.Empty;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password has been reset successfully. Please login with your new password.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Auth/CheckEmailAvailability
        [HttpGet]
        public async Task<IActionResult> CheckEmailAvailability(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Json(new { available = false, message = "Email is required" });

            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            return Json(new { available = !exists, message = exists ? "Email already registered" : "Email available" });
        }
    }
}