using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.User
{
    public class SubscriptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
