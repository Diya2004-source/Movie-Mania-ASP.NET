using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
