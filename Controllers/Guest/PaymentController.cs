using Microsoft.AspNetCore.Mvc;

namespace MovieMania.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Subscribe()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}