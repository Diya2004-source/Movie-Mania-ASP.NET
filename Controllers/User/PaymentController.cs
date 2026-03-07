using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.User
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", "User");
        }
    }
}
