using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;

namespace MovieMania.Controllers.Admin
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Use async methods for better performance
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