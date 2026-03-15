using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Guest
{
    [Route("Payment")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> Plans()
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            return View("~/Views/Guest/Payment/Plans.cshtml", plans);
        }

        [HttpGet("checkout")]
        public async Task<IActionResult> Checkout(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
                return NotFound();

            // If user is already logged in, redirect to user payment
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Process", "Payment", new { area = "User", planId = id });
            }

            ViewBag.Plan = plan;
            return View("~/Views/Guest/Payment/Checkout.cshtml");
        }

        [HttpPost("process-guest")]
        public IActionResult ProcessGuest(int planId, string email, string paymentMethod)
        {
            // Store payment info in TempData
            TempData["PlanId"] = planId;
            TempData["Email"] = email;
            TempData["PaymentMethod"] = paymentMethod;
            TempData["Success"] = "Please login to complete your payment.";

            return RedirectToAction("Login", "Auth", new { area = "Guest" });
        }
    }
}