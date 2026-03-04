using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.Guest
{
    public class GuestMoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
