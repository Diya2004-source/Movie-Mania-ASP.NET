using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.User
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
