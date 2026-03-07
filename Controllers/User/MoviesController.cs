using Microsoft.AspNetCore.Mvc;
using MovieMania.Models;
using System.Linq;

namespace MovieMania.Controllers.User
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Watch(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);

            return View(movie);
        }
    }
}