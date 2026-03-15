using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminSubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSubscriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminSubscriptions
        public async Task<IActionResult> Index()
        {
            var plans = await _context.SubscriptionPlans
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            return View("~/Views/Admin/Subscriptions/Index.cshtml", plans);
        }

        // GET: AdminSubscriptions/Create
        public IActionResult Create()
        {
            return View("~/Views/Admin/Subscriptions/Create.cshtml");
        }

        // POST: AdminSubscriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriptionPlan plan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View("~/Views/Admin/Subscriptions/Create.cshtml", plan);
                }

                plan.CreatedAt = DateTime.Now;
                plan.IsActive = true;

                await _context.SubscriptionPlans.AddAsync(plan);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Plan '{plan.Name}' created successfully! Price: ₹{plan.Price:N2}";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "❌ Plan was not created.";
                return View("~/Views/Admin/Subscriptions/Create.cshtml", plan);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                return View("~/Views/Admin/Subscriptions/Create.cshtml", plan);
            }
        }

        // GET: AdminSubscriptions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                TempData["Error"] = $"❌ Plan with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Subscriptions/Edit.cshtml", plan);
        }

        // POST: AdminSubscriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubscriptionPlan plan)
        {
            if (id != plan.Id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View("~/Views/Admin/Subscriptions/Edit.cshtml", plan);
                }

                var existingPlan = await _context.SubscriptionPlans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingPlan == null)
                {
                    TempData["Error"] = $"❌ Plan with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Preserve original CreatedAt
                plan.CreatedAt = existingPlan.CreatedAt;
                plan.UpdatedAt = DateTime.Now;

                _context.SubscriptionPlans.Update(plan);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Plan '{plan.Name}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "❌ No changes were saved.";
                return View("~/Views/Admin/Subscriptions/Edit.cshtml", plan);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                return View("~/Views/Admin/Subscriptions/Edit.cshtml", plan);
            }
        }

        // GET: AdminSubscriptions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                TempData["Error"] = $"❌ Plan with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Subscriptions/Delete.cshtml", plan);
        }

        // POST: AdminSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var plan = await _context.SubscriptionPlans.FindAsync(id);
                if (plan == null)
                {
                    TempData["Error"] = $"❌ Plan with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.SubscriptionPlans.Remove(plan);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Plan '{plan.Name}' deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ Plan was not deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AdminSubscriptions/ToggleStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var plan = await _context.SubscriptionPlans.FindAsync(id);
                if (plan == null)
                {
                    return Json(new { success = false, message = "Plan not found" });
                }

                plan.IsActive = !plan.IsActive;
                plan.UpdatedAt = DateTime.Now;

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var status = plan.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"✅ Plan '{plan.Name}' {status} successfully!",
                        isActive = plan.IsActive
                    });
                }

                return Json(new { success = false, message = "❌ No changes were saved." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"❌ Error: {ex.Message}" });
            }
        }

        // GET: AdminSubscriptions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                TempData["Error"] = $"❌ Plan with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Subscriptions/Details.cshtml", plan);
        }
    }
}