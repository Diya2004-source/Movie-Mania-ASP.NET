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
<<<<<<< HEAD

//return RedirectToAction("Index", "Home", new { area = "User" });
=======
      
//returning RedirectToAction("Index", "Home", new { area = "User" });
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
