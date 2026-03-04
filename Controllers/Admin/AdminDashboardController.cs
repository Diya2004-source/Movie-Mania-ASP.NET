using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            ViewBag.TotalMovies = _context.Movies.Count();
            ViewBag.TotalUsers = _context.Users.Count();

            ViewBag.ActiveSubscriptions =
                _context.UserSubscriptions.Count(s => s.EndDate >= DateTime.Now);

            ViewBag.TotalRevenue =
                _context.Payments.Sum(p => (decimal?)p.Amount) ?? 0;

            ViewBag.RecentMovies = _context.Movies
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToList();

            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}