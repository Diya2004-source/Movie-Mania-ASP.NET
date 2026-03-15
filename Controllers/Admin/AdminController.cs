using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MovieMania.Controllers.Admin
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            // If already logged in as admin, go to dashboard
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }
            return RedirectToAction("Login");
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in as admin, go to dashboard
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }

            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View("~/Views/Admin/Login.cshtml");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            // Simple hardcoded check (you can replace with database check later)
            if (email == "admin@moviemania.com" && password == "Admin@123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Email, email ?? string.Empty),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("AdminId", "1"),
                    new Claim("LoginTime", DateTime.Now.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["Success"] = "Welcome back, Admin!";

                // Redirect to return URL or dashboard
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }

            ViewBag.Error = "Invalid email or password";
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View("~/Views/Admin/Login.cshtml");
        }

        [HttpPost("Logout")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        [HttpGet("Logout")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View("~/Views/Admin/AccessDenied.cshtml");
        }

        [HttpGet("Profile")]
        [Authorize(Roles = "admin")]
        public IActionResult Profile()
        {
            var userName = User.Identity?.Name ?? "Admin";
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "admin@moviemania.com";
            var loginTime = User.FindFirst("LoginTime")?.Value ?? DateTime.Now.ToString();

            ViewBag.UserName = userName;
            ViewBag.Email = email;
            ViewBag.LoginTime = loginTime;

            return View("~/Views/Admin/Profile.cshtml");
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Content($"✅ Logged in as: {User.Identity.Name ?? "Unknown"}<br>" +
                              $"Role: Admin? {User.IsInRole("admin")}<br>" +
                              $"Authentication Type: {User.Identity.AuthenticationType ?? "None"}<br>" +
                              $"Is Authenticated: {User.Identity.IsAuthenticated}");
            }
            return Content("❌ Not logged in");
        }

        [HttpGet("Settings")]
        [Authorize(Roles = "admin")]
        public IActionResult Settings()
        {
            return View("~/Views/Admin/Settings.cshtml");
        }

        [HttpPost("ChangePassword")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["Error"] = "All fields are required";
                return RedirectToAction("Profile");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New password and confirm password do not match";
                return RedirectToAction("Profile");
            }

            // Here you would verify current password and update in database
            // For now, just show success message
            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction("Profile");
        }
    }
}