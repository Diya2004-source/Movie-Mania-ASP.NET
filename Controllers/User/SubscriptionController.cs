using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

            // Get current active subscription
            var currentSubscription = await _context.UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.UserId == userId && s.IsActive)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            // Get subscription history of user
            var subscriptionHistory = await _context.UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartDate)
                .Skip(currentSubscription != null ? 1 : 0)
                .Take(5)
                .ToListAsync();

            // Get available plans
            var availablePlans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            ViewBag.CurrentSubscription = currentSubscription;
            ViewBag.SubscriptionHistory = subscriptionHistory;

            return View("~/Views/User/Subscription/Index.cshtml", availablePlans);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Json(new { success = false, message = "User not found" });

            var userId = int.Parse(userIdClaim.Value);

            var subscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

            if (subscription == null)
                return Json(new { success = false, message = "No active subscription found" });

            subscription.IsActive = false;
            // Remove UpdatedAt if it doesn't exist in your model
            // subscription.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Subscription cancelled successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

            var subscription = await _context.UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (subscription == null)
                return NotFound();

            // Get payments related to this subscription
            var payments = await _context.Payments
                .Where(p => p.UserId == userId && p.SubscriptionPlanId == subscription.PlanId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            ViewBag.Payments = payments;

            return View("~/Views/User/Subscription/Details.cshtml", subscription);
        }
    }
}