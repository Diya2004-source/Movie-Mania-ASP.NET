using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Subscriptions")]
    public class AdminSubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSubscriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Subscriptions
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var plans = await _context.SubscriptionPlans
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            return View("~/Views/Admin/Subscriptions/Index.cshtml", plans);
        }

        // GET: Admin/Subscriptions/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Subscriptions/Create.cshtml");
        }

        // POST: Admin/Subscriptions/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriptionPlan plan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"❌ Validation failed: <br/>{errorMessage}";
                    return View("~/Views/Admin/Subscriptions/Create.cshtml", plan);
                }

                plan.CreatedAt = DateTime.Now;
                await _context.SubscriptionPlans.AddAsync(plan);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Plan '{plan.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ Plan was not created. Please try again.";
                    return View("~/Views/Admin/Subscriptions/Create.cshtml", plan);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                return View("~/Views/Admin/Subscriptions/Create.cshtml", plan);
            }
        }

        // GET: Admin/Subscriptions/Edit/5
        [HttpGet("Edit/{id}")]
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

        // POST: Admin/Subscriptions/Edit/5
        [HttpPost("Edit/{id}")]
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
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"❌ Validation failed: <br/>{errorMessage}";
                    return View("~/Views/Admin/Subscriptions/Edit.cshtml", plan);
                }

                var existingPlan = await _context.SubscriptionPlans.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (existingPlan == null)
                {
                    TempData["Error"] = $"❌ Plan with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                plan.CreatedAt = existingPlan.CreatedAt;
                plan.UpdatedAt = DateTime.Now;

                _context.SubscriptionPlans.Update(plan);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Plan '{plan.Name}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ No changes were saved.";
                    return View("~/Views/Admin/Subscriptions/Edit.cshtml", plan);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                return View("~/Views/Admin/Subscriptions/Edit.cshtml", plan);
            }
        }

        // GET: Admin/Subscriptions/Delete/5
        [HttpGet("Delete/{id}")]
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

        // POST: Admin/Subscriptions/Delete/5
        [HttpPost("Delete/{id}")]
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
                int result = await _context.SaveChangesAsync();

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

        // POST: Admin/Subscriptions/ToggleStatus/5
        [HttpPost("ToggleStatus/{id}")]
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
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string status = plan.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"✅ Plan '{plan.Name}' {status} successfully!",
                        isActive = plan.IsActive
                    });
                }
                else
                {
                    return Json(new { success = false, message = "❌ No changes were saved." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"❌ Error: {ex.Message}" });
            }
        }

        // GET: Admin/Subscriptions/Details/5
        [HttpGet("Details/{id}")]
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