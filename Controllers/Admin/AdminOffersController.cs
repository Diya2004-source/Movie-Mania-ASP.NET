using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Offers")]
    public class AdminOffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: /Admin/Offers
        // =========================
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .Include(o => o.SubscriptionPlan)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View("~/Views/Admin/Offers/Index.cshtml", offers);
        }

        // =========================
        // GET: /Admin/Offers/Create
        // =========================
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            return View("~/Views/Admin/Offers/Create.cshtml");
        }

        // =========================
        // POST: /Admin/Offers/Create
        // =========================
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Offer offer)
        {
            try
            {
                // Check if model state is valid
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"❌ Validation failed: <br/>{errorMessage}";

                    ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                        .Where(p => p.IsActive)
                        .ToListAsync();
                    return View("~/Views/Admin/Offers/Create.cshtml", offer);
                }

                // Validate dates
                if (offer.StartDate >= offer.EndDate)
                {
                    TempData["Error"] = "❌ End date must be after start date";
                    ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                        .Where(p => p.IsActive)
                        .ToListAsync();
                    return View("~/Views/Admin/Offers/Create.cshtml", offer);
                }

                // Set default values
                offer.CreatedAt = DateTime.Now;
                offer.CurrentUses = 0;

                // Add to database
                await _context.Offers.AddAsync(offer);
                int result = await _context.SaveChangesAsync();

                // Check if save was successful
                if (result > 0)
                {
                    TempData["Success"] = $"✅ Offer '{offer.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ Offer was not created. Please try again.";
                    ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                        .Where(p => p.IsActive)
                        .ToListAsync();
                    return View("~/Views/Admin/Offers/Create.cshtml", offer);
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database-specific errors
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                TempData["Error"] = $"❌ Database error: {innerMessage}";

                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View("~/Views/Admin/Offers/Create.cshtml", offer);
            }
            catch (Exception ex)
            {
                // Handle any other errors
                TempData["Error"] = $"❌ Error: {ex.Message}";

                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View("~/Views/Admin/Offers/Create.cshtml", offer);
            }
        }

        // =========================
        // GET: /Admin/Offers/Edit/5
        // =========================
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                TempData["Error"] = $"❌ Offer with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            return View("~/Views/Admin/Offers/Edit.cshtml", offer);
        }

        // =========================
        // POST: /Admin/Offers/Edit/5
        // =========================
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Offer offer)
        {
            if (id != offer.Id)
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

                    ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                        .Where(p => p.IsActive)
                        .ToListAsync();
                    return View("~/Views/Admin/Offers/Edit.cshtml", offer);
                }

                // Validate dates
                if (offer.StartDate >= offer.EndDate)
                {
                    TempData["Error"] = "❌ End date must be after start date";
                    ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                        .Where(p => p.IsActive)
                        .ToListAsync();
                    return View("~/Views/Admin/Offers/Edit.cshtml", offer);
                }

                // Get existing offer to preserve some fields
                var existingOffer = await _context.Offers.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
                if (existingOffer == null)
                {
                    TempData["Error"] = $"❌ Offer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                offer.CreatedAt = existingOffer.CreatedAt; // Preserve original creation date
                offer.UpdatedAt = DateTime.Now;

                _context.Offers.Update(offer);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Offer '{offer.Title}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ No changes were saved.";
                    ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                        .Where(p => p.IsActive)
                        .ToListAsync();
                    return View("~/Views/Admin/Offers/Edit.cshtml", offer);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Offers.Any(o => o.Id == offer.Id))
                {
                    TempData["Error"] = $"❌ Offer with ID {id} not found.";
                }
                else
                {
                    TempData["Error"] = "❌ Concurrency error. Please try again.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View("~/Views/Admin/Offers/Edit.cshtml", offer);
            }
        }

        // =========================
        // GET: /Admin/Offers/Delete/5
        // =========================
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                TempData["Error"] = $"❌ Offer with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Offers/Delete.cshtml", offer);
        }

        // =========================
        // POST: /Admin/Offers/Delete/5
        // =========================
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var offer = await _context.Offers.FindAsync(id);
                if (offer == null)
                {
                    TempData["Error"] = $"❌ Offer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Offers.Remove(offer);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Offer '{offer.Title}' deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ Offer was not deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error deleting offer: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // POST: /Admin/Offers/ToggleStatus/5
        // =========================
        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var offer = await _context.Offers.FindAsync(id);
                if (offer == null)
                {
                    return Json(new { success = false, message = "Offer not found" });
                }

                offer.IsActive = !offer.IsActive;
                offer.UpdatedAt = DateTime.Now;

                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string status = offer.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"✅ Offer '{offer.Title}' {status} successfully!",
                        isActive = offer.IsActive
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
    }
}