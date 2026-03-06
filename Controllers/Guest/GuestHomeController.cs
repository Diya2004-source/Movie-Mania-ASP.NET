using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System.Linq;

namespace MovieMania.Controllers
{
    public class GuestHomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestHomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View("~/Views/Guest/Home/Index.cshtml", movies);
        }
    }
}