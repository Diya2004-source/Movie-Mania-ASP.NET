using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            //return RedirectToAction("Index", "Home", new { controller = "Home", area = "User" });
            return View();
        }
    }
}

//return RedirectToAction("Index", "Home", new { area = "User" });