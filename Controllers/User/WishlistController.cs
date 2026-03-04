using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.User
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
