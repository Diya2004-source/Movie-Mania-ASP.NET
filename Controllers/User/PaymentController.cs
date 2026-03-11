// Controllers/User/PaymentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Payment/Checkout
        public async Task<IActionResult> Checkout(int? subscriptionId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (subscriptionId.HasValue)
            {
                var subscription = await _context.UserSubscriptions
                    .Include(us => us.SubscriptionPlan)
                    .FirstOrDefaultAsync(us => us.Id == subscriptionId && us.UserId == userId);

                if (subscription == null)
                {
                    return NotFound();
                }

                var viewModel = new CheckoutViewModel
                {
                    SubscriptionId = subscription.Id,
                    PlanName = subscription.SubscriptionPlan.Name,
                    Amount = subscription.SubscriptionPlan.Price,
                    UserEmail = User.FindFirstValue(ClaimTypes.Email),
                    UserName = User.FindFirstValue(ClaimTypes.Name)
                };

                return View(viewModel);
            }

            return RedirectToAction("Subscription", "Profile");
        }

        // POST: User/Payment/Process
        [HttpPost]
        public async Task<IActionResult> Process([FromBody] PaymentProcessViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!IsValidPaymentMethod(model))
            {
                return Json(new { success = false, message = "Invalid payment details" });
            }

            var subscription = await _context.UserSubscriptions
                .Include(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync(us => us.Id == model.SubscriptionId && us.UserId == userId);

            if (subscription == null)
            {
                return Json(new { success = false, message = "Subscription not found" });
            }

            var paymentSuccess = await ProcessPaymentWithGateway(model);

            if (!paymentSuccess)
            {
                return Json(new { success = false, message = "Payment failed. Please try again." });
            }

            var payment = new Payment
            {
                UserId = userId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,  // Now this exists
                Amount = subscription.SubscriptionPlan.Price,
                PaymentDate = DateTime.Now,
                PaymentMethod = model.PaymentMethod,
                TransactionId = GenerateTransactionId(),
                Status = "Completed",
                PaymentDetails = System.Text.Json.JsonSerializer.Serialize(model)
            };

            _context.Payments.Add(payment);

            subscription.PaymentStatus = "Paid";
            subscription.Status = "Active";
            subscription.PaymentReference = payment.TransactionId;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Payment successful! 🎉",
                redirectUrl = Url.Action("Confirmation", new { id = payment.Id })
            });
        }

        // GET: User/Payment/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var payment = await _context.Payments
                .Include(p => p.SubscriptionPlan)  // Now this works
                .Include(p => p.User)  // This now references AppUser
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // Helper methods
        private bool IsValidPaymentMethod(PaymentProcessViewModel model)
        {
            if (string.IsNullOrEmpty(model.PaymentMethod))
                return false;

            return model.PaymentMethod.ToLower() switch
            {
                "card" => !string.IsNullOrEmpty(model.CardNumber) &&
                          model.CardNumber.Length >= 15 &&
                          !string.IsNullOrEmpty(model.CardExpiry) &&
                          !string.IsNullOrEmpty(model.CardCvv) &&
                          model.CardCvv.Length >= 3,

                "upi" => !string.IsNullOrEmpty(model.UpiId) &&
                         model.UpiId.Contains("@"),

                "netbanking" => !string.IsNullOrEmpty(model.BankName) &&
                                !string.IsNullOrEmpty(model.AccountNumber),

                _ => false
            };
        }

        private async Task<bool> ProcessPaymentWithGateway(PaymentProcessViewModel model)
        {
            await Task.Delay(2000);
            return new Random().Next(1, 100) <= 90;
        }

        private string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.Ticks.ToString() +
                   new Random().Next(1000, 9999).ToString();
        }
    }
}