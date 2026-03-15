using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Your existing code
            ViewBag.TotalMovies = await _context.Movies.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.ActiveSubscriptions = await _context.UserSubscriptions
                .CountAsync(s => s.EndDate >= DateTime.Now);
            ViewBag.TotalRevenue = await _context.Payments
                .SumAsync(p => (decimal?)p.Amount) ?? 0;
            ViewBag.RecentMovies = await _context.Movies
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}