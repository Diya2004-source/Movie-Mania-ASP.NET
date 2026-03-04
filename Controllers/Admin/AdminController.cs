using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace movie_mania.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (Email == "admin@gmail.com" && Password == "123456")
            {
                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Email),
                    new Claim(ClaimTypes.Role, "admin")  // IMPORTANT
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                // Sign in
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Index", "AdminDashboard");
            }

            ViewBag.Error = "Invalid Email or Password!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}