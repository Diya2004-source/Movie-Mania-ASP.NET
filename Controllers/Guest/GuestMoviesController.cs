using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.Guest
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