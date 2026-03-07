//using Microsoft.AspNetCore.Mvc;
//using MovieMania.Models;
//using System.Linq;

//namespace MovieMania.Controllers.Guest
//{
//    public class GuestMoviesController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public GuestMoviesController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // Movies Page
//        public IActionResult Movies()
//        {
//            var movies = _context.Movies.ToList();
//            return View("~/Views/Guest/Movies/Movies.cshtml", movies);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;

namespace MovieMania.Controllers
{
    public class GuestMoviesController : Controller
    {
        public IActionResult Movies()
        {
            return View("~/Views/Guest/Movies/Movies.cshtml");
        }
    public IActionResult Shows()
        {
            return View("~/Views/Guest/Movies/Shows.cshtml");
        }

        public IActionResult Anime()
        {
            return View("~/Views/Guest/Movies/Anime.cshtml");
        }
    }

}
