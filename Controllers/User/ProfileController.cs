<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.ComponentModel.DataAnnotations;
<<<<<<< HEAD
=======
=======
﻿//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MovieMania.Models;
//using System.ComponentModel.DataAnnotations;
//using System.Security.Claims;

//namespace MovieMania.Controllers.User
//{
//    [Authorize(Roles = "user")]
//    public class ProfileController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public ProfileController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//            if (userIdClaim == null)
//                return RedirectToAction("Login", "Auth");

//            var userId = int.Parse(userIdClaim.Value);

//            var user = await _context.Users.FindAsync(userId);
//            if (user == null)
//                return NotFound();

//            // Get user's active subscription from database
//            var activeSubscription = await _context.UserSubscriptions
//                .Include(s => s.SubscriptionPlan)
//                .Where(s => s.UserId == userId && s.IsActive)
//                .FirstOrDefaultAsync();

//            ViewBag.ActiveSubscription = activeSubscription;

//            return View("~/Views/User/Profile/Index.cshtml", user);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Update(AppUser model)
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//            if (userIdClaim == null)
//                return RedirectToAction("Login", "Auth");

//            var userId = int.Parse(userIdClaim.Value);

//            if (userId != model.Id)
//                return NotFound();

//            var user = await _context.Users.FindAsync(userId);
//            if (user == null)
//                return NotFound();

//            // Update only allowed fields
//            user.Name = model.Name;

//            // Check if UpdatedAt property exists before using it
//            // If your AppUser model has UpdatedAt property, uncomment this:
//            // user.UpdatedAt = DateTime.Now;

//            await _context.SaveChangesAsync();
//            TempData["Success"] = "Profile updated successfully";

//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//            if (userIdClaim == null)
//                return RedirectToAction("Login", "Auth");

//            var userId = int.Parse(userIdClaim.Value);

//            if (!ModelState.IsValid)
//                return View(model);

//            var user = await _context.Users.FindAsync(userId);
//            if (user == null)
//                return NotFound();

//            // Verify current password
//            if (user.Password != model.CurrentPassword)
//            {
//                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
//                return View(model);
//            }

//            // Update password of user
//            user.Password = model.NewPassword;

//            // Check if UpdatedAt property exists
//            // if (user.GetType().GetProperty("UpdatedAt") != null)
//            // {
//            //     user.UpdatedAt = DateTime.Now;
//            // }

//            await _context.SaveChangesAsync();
//            TempData["Success"] = "Password changed successfully";

//            return RedirectToAction(nameof(Index));
//        }
//    }

//    public class ChangePasswordViewModel
//    {
//        [Required]
//        [DataType(DataType.Password)]
//        [Display(Name = "Current Password")]
//        public string CurrentPassword { get; set; } = string.Empty;

//        [Required]
//        [StringLength(100, MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "New Password")]
//        public string NewPassword { get; set; } = string.Empty;

//        [DataType(DataType.Password)]
//        [Display(Name = "Confirm New Password")]
//        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
//        public string ConfirmPassword { get; set; } = string.Empty;
//    }
//}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
<<<<<<< HEAD
=======
=======
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Get user's active subscription
            var activeSubscription = await _context.UserSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.UserId == userId && s.IsActive)
                .FirstOrDefaultAsync();

            ViewBag.ActiveSubscription = activeSubscription;

            return View("~/Views/User/Profile/Index.cshtml", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AppUser model)
<<<<<<< HEAD
=======
=======
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
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
            if (userId != model.Id)
                return NotFound();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Update only allowed fields
            user.Name = model.Name;

            // Check if UpdatedAt property exists before using it
            // If your AppUser model has UpdatedAt property, uncomment this:
            // user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Profile updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth");

            var userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Verify current password
            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                return View(model);
            }

            // Update password
            user.Password = model.NewPassword;

            // Check if UpdatedAt property exists
            // if (user.GetType().GetProperty("UpdatedAt") != null)
            // {
            //     user.UpdatedAt = DateTime.Now;
            // }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Password changed successfully";

            return RedirectToAction(nameof(Index));
        }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
<<<<<<< HEAD
=======
=======
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
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
    }
}