using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
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

        // GET: User/Subscription/Plans
        public async Task<IActionResult> Plans()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            var plans = await _context.SubscriptionPlans
                .Where(sp => sp.IsActive)
                .OrderBy(sp => sp.Price)
                .ToListAsync();

            var currentSubscription = await _context.UserSubscriptions
                .Include(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync(us => us.UserId == userId && us.Status == "Active");

            ViewBag.CurrentSubscription = currentSubscription;

            return View(plans);
        }

        // POST: User/Subscription/Cancel
        [HttpPost]
        public async Task<IActionResult> Cancel()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var subscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(us => us.UserId == userId && us.Status == "Active");

            if (subscription == null)
            {
                return Json(new { success = false, message = "No active subscription found" });
            }

            subscription.Status = "Cancelled";
            subscription.EndDate = DateTime.Now;
            subscription.CancelledAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Subscription cancelled successfully",
                endDate = subscription.EndDate.ToString("MMM dd, yyyy")
            });
        }

        // POST: User/Subscription/Reactivate
        [HttpPost]
        public async Task<IActionResult> Reactivate()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var subscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(us => us.UserId == userId && us.Status == "Cancelled");

            if (subscription == null)
            {
                return Json(new { success = false, message = "No cancelled subscription found" });
            }

            subscription.Status = "Active";
            subscription.ReactivatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Subscription reactivated successfully" });
        }
    }
}