using Microsoft.AspNetCore.Mvc;

namespace MovieMania.Controllers
{
    public class GuestHomeController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Guest/Home/Index.cshtml");
        }
    }
}