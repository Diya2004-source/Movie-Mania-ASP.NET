using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Offers")]
    public class AdminOffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _viewPath = "~/Views/Admin/Offers/";

        public AdminOffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .Include(o => o.SubscriptionPlan)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(_viewPath + "Index.cshtml", offers);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            return View(_viewPath + "Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Offer offer)
        {
            if (offer.StartDate >= offer.EndDate)
                ModelState.AddModelError("EndDate", "End date must be after start date");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br/>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["Error"] = $"Validation failed:<br/>{errors}";
                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View(_viewPath + "Create.cshtml", offer);
            }

            try
            {
                offer.CreatedAt = DateTime.Now;
                offer.CurrentUses = 0;

                await _context.Offers.AddAsync(offer);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"Offer '{offer.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Offer was not created.";
                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View(_viewPath + "Create.cshtml", offer);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View(_viewPath + "Create.cshtml", offer);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                TempData["Error"] = $"Offer with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            return View(_viewPath + "Edit.cshtml", offer);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Offer offer)
        {
            if (id != offer.Id)
                return NotFound();

            if (offer.StartDate >= offer.EndDate)
                ModelState.AddModelError("EndDate", "End date must be after start date");

            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br/>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["Error"] = $"Validation failed:<br/>{errors}";
                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View(_viewPath + "Edit.cshtml", offer);
            }

            try
            {
                var existingOffer = await _context.Offers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (existingOffer == null)
                {
                    TempData["Error"] = $"Offer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                offer.CreatedAt = existingOffer.CreatedAt;
                offer.UpdatedAt = DateTime.Now;

                _context.Update(offer);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"Offer '{offer.Title}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No changes were saved.";
                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View(_viewPath + "Edit.cshtml", offer);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.SubscriptionPlans = await _context.SubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View(_viewPath + "Edit.cshtml", offer);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                TempData["Error"] = $"Offer with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(_viewPath + "Delete.cshtml", offer);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var offer = await _context.Offers.FindAsync(id);
                if (offer == null)
                {
                    TempData["Error"] = $"Offer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Offers.Remove(offer);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    TempData["Success"] = $"Offer '{offer.Title}' deleted successfully!";
                else
                    TempData["Error"] = "Offer was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting offer: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.SubscriptionPlan)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offer == null)
            {
                TempData["Error"] = $"Offer with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Details.cshtml", offer);
        }

        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var offer = await _context.Offers.FindAsync(id);
                if (offer == null)
                    return Json(new { success = false, message = "Offer not found" });

                offer.IsActive = !offer.IsActive;
                offer.UpdatedAt = DateTime.Now;

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var status = offer.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"Offer '{offer.Title}' {status} successfully!",
                        isActive = offer.IsActive
                    });
                }

                return Json(new { success = false, message = "No changes were saved." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}