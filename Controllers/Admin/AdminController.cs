using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;  // Change from movie_mania.Models
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MovieMania.Controllers.Admin  // Change from movie_mania.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in as admin, redirect to dashboard
            if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "AdminDashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "Email and Password are required";
                return View();
            }

            // Check if user exists in database with admin role
            var admin = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Email &&
                                         u.Role == "admin" &&
                                         u.IsActive);

            if (admin == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // In production, verify password hash
            // For now, using direct comparison (you should hash passwords in production)
            if (admin.Password != Password)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // Update last login
            admin.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Name),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, admin.Role),  // "admin"
                new Claim("ProfilePicture", admin.ProfilePictureUrl ?? "")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            // Sign in
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });

            return RedirectToAction("Index", "AdminDashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        // Optional: Method to create first admin (run once)
        public async Task<IActionResult> CreateFirstAdmin()
        {
            // Check if any admin exists
            var adminExists = await _context.Users
                .AnyAsync(u => u.Role == "admin");

            if (!adminExists)
            {
                var admin = new AppUser
                {
                    Name = "Administrator",
                    Email = "admin@moviemania.com",
                    Password = "Admin@123", // In production, hash this
                    Role = "admin",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(admin);
                await _context.SaveChangesAsync();

                return Content("Admin created successfully! Email: admin@moviemania.com, Password: Admin@123");
            }

            return Content("Admin already exists!");
        }
    }
}