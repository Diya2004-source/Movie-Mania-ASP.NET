//using Microsoft.AspNetCore.Mvc;

//namespace MovieMania.Controllers.Guest
//{
//    public class AuthController : Controller
//    {
//        public IActionResult Login()
//        {
//            return View("~/Views/Guest/Auth/Login.cshtml");
//        }

//        public IActionResult Register()
//        {
//            return View("~/Views/Guest/Auth/Register.cshtml");
//        }
//    }


using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.Guest
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View("~/Views/Guest/Auth/Login.cshtml");
        }

        public IActionResult Register()
        {
            return View("~/Views/Guest/Auth/Register.cshtml");
        }
    }
}