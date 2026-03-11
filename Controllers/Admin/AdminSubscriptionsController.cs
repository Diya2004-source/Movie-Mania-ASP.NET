using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
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
            return View(plans);
        }

        // GET: AdminSubscriptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminSubscriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriptionPlan plan)
        {
            if (ModelState.IsValid)
            {
                plan.CreatedAt = DateTime.Now;
                _context.Add(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subscription plan created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: AdminSubscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
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

            if (ModelState.IsValid)
            {
                try
                {
                    plan.UpdatedAt = DateTime.Now;
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Subscription plan updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionPlanExists(plan.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: AdminSubscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: AdminSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan != null)
            {
                _context.SubscriptionPlans.Remove(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subscription plan deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionPlanExists(int id)
        {
            return _context.SubscriptionPlans.Any(e => e.Id == id);
        }

        // GET: AdminSubscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }
    }
}