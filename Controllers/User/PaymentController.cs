using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    [Route("User/[controller]")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth", new { area = "Guest" });

            var userId = int.Parse(userIdClaim.Value);

            var payments = await _context.Payments
                .Include(p => p.SubscriptionPlan)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View("~/Views/User/Payment/History.cshtml", payments);
        }

        [HttpGet("process")]
        public async Task<IActionResult> Process(int planId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth", new { area = "Guest" });

            var userId = int.Parse(userIdClaim.Value);
            var plan = await _context.SubscriptionPlans.FindAsync(planId);

            if (plan == null)
                return NotFound();

            // Check if user already has active subscription
            var existingSubscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

            ViewBag.ExistingSubscription = existingSubscription;
            ViewBag.Plan = plan;

            return View("~/Views/User/Payment/Process.cshtml");
        }

        [HttpPost("complete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int planId, string paymentMethod)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Json(new { success = false, message = "User not logged in" });

            var userId = int.Parse(userIdClaim.Value);
            var plan = await _context.SubscriptionPlans.FindAsync(planId);

            if (plan == null)
                return Json(new { success = false, message = "Plan not found" });

<<<<<<< HEAD
            // Create payment record
=======
<<<<<<< HEAD
            // Create payment record
=======
            // Create payment record 
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
            var payment = new Payment
            {
                UserId = userId,
                SubscriptionPlanId = planId,
                Amount = plan.Price,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.Now,
                Status = "completed",
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDetails = $"Payment for {plan.Name} plan",
                CreatedAt = DateTime.Now
            };

            _context.Payments.Add(payment);

            // Deactivate old active subscription if exists
            var existingSubscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

            if (existingSubscription != null)
            {
                existingSubscription.IsActive = false;
            }

<<<<<<< HEAD
            // Create new subscription
=======
<<<<<<< HEAD
            // Create new subscription
=======
            // Create new subscription of user
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
            var subscription = new UserSubscription
            {
                UserId = userId,
                PlanId = planId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(plan.DurationInDays),
                IsActive = true,
                PaymentStatus = "paid",
                CreatedAt = DateTime.Now
            };

            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Payment successful! Your subscription is now active.";

            return Json(new
            {
                success = true,
                message = "Payment successful! Subscription activated.",
                redirectUrl = Url.Action("Index", "Home")
            });
        }

        [HttpGet("invoice/{id}")]
        public async Task<IActionResult> Invoice(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth", new { area = "Guest" });

            var userId = int.Parse(userIdClaim.Value);

            var payment = await _context.Payments
                .Include(p => p.SubscriptionPlan)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (payment == null)
                return NotFound();

            return View("~/Views/User/Payment/Invoice.cshtml", payment);
        }
    }
}