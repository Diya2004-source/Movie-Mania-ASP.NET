using Microsoft.AspNetCore.Mvc;

namespace movie.Controllers.User
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
