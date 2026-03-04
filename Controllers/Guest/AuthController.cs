using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.Guest
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
