using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.User
{
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
