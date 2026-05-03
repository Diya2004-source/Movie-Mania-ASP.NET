using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Security.Claims;
using System.Linq;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Code to GET: /User/Home
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth", new { area = "Guest" });

            var userId = int.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(userId);

            ViewBag.UserName = user?.Name ?? "User";

            // Build a dashboard view model rather than using ViewBag
            var dashboard = new DashboardViewModel
            {
                Movies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(10)
                    .ToListAsync(),

                Shows = await _context.Shows
                    .Include(s => s.Episodes)
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(10)
                    .ToListAsync(),

                Anime = await _context.Movies
                    .Where(m => m.IsActive && m.Genre != null && m.Genre.ToLower() == "anime")
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(10)
                    .ToListAsync()
            };

            return View("~/Views/User/Home/Index.cshtml", dashboard);
        }
    }

    // ViewModel for dashboard
    public class DashboardViewModel
    {
        public List<Movie> Movies { get; set; } = new();
        public List<Show> Shows { get; set; } = new();
        public List<Movie> Anime { get; set; } = new();
    }
}