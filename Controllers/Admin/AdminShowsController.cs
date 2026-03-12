using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Shows")]
    public class AdminShowsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string viewFolder = "~/Views/Admin/Shows/";

        public AdminShowsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =========================
        // GET: Admin/Shows
        // =========================
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var shows = await _context.Shows
                .Include(s => s.Episodes)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(viewFolder + "Index.cshtml", shows);
        }

        // =========================
        // GET: Admin/Shows/Create
        // =========================
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
            return View(viewFolder + "Create.cshtml");
        }

        // =========================
        // POST: Admin/Shows/Create
        // =========================
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Show show, IFormFile? ThumbnailFile, IFormFile? PosterFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"❌ Validation failed: <br/>{errorMessage}";

                    ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
                    return View(viewFolder + "Create.cshtml", show);
                }

                // Handle thumbnail upload
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "shows", "thumbnails");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }

                    show.ThumbnailUrl = "/uploads/shows/thumbnails/" + uniqueFileName;
                }

                // Handle poster upload
                if (PosterFile != null && PosterFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "shows", "posters");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + PosterFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterFile.CopyToAsync(fileStream);
                    }

                    show.PosterUrl = "/uploads/shows/posters/" + uniqueFileName;
                }

                show.CreatedAt = DateTime.Now;
                show.ViewsCount = 0;

                await _context.Shows.AddAsync(show);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Show '{show.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ Show was not created. Please try again.";
                    ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
                    return View(viewFolder + "Create.cshtml", show);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
                return View(viewFolder + "Create.cshtml", show);
            }
        }

        // =========================
        // GET: Admin/Shows/Edit/5
        // =========================
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            if (show == null)
            {
                TempData["Error"] = $"❌ Show with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
            return View(viewFolder + "Edit.cshtml", show);
        }

        // =========================
        // POST: Admin/Shows/Edit/5
        // =========================
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Show show, IFormFile? ThumbnailFile, IFormFile? PosterFile)
        {
            if (id != show.Id)
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

                    ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
                    return View(viewFolder + "Edit.cshtml", show);
                }

                var existingShow = await _context.Shows.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                if (existingShow == null)
                {
                    TempData["Error"] = $"❌ Show with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Handle thumbnail upload
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "shows", "thumbnails");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }

                    show.ThumbnailUrl = "/uploads/shows/thumbnails/" + uniqueFileName;
                }

                // Handle poster upload
                if (PosterFile != null && PosterFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "shows", "posters");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + PosterFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterFile.CopyToAsync(fileStream);
                    }

                    show.PosterUrl = "/uploads/shows/posters/" + uniqueFileName;
                }

                show.CreatedAt = existingShow.CreatedAt;
                show.ViewsCount = existingShow.ViewsCount;
                show.UpdatedAt = DateTime.Now;

                _context.Shows.Update(show);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Show '{show.Title}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ No changes were saved.";
                    ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
                    return View(viewFolder + "Edit.cshtml", show);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                ViewBag.Genres = await _context.Genres.Where(g => g.IsActive).ToListAsync();
                return View(viewFolder + "Edit.cshtml", show);
            }
        }

        // =========================
        // GET: Admin/Shows/Delete/5
        // =========================
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
            {
                TempData["Error"] = $"❌ Show with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Delete.cshtml", show);
        }

        // =========================
        // POST: Admin/Shows/Delete/5
        // =========================
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var show = await _context.Shows
                    .Include(s => s.Episodes)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (show == null)
                {
                    TempData["Error"] = $"❌ Show with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (show.Episodes != null && show.Episodes.Any())
                {
                    _context.Episodes.RemoveRange(show.Episodes);
                }

                _context.Shows.Remove(show);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Show '{show.Title}' and its episodes deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ Show was not deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error deleting show: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // POST: Admin/Shows/ToggleStatus/5
        // =========================
        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var show = await _context.Shows.FindAsync(id);
                if (show == null)
                {
                    return Json(new { success = false, message = "Show not found" });
                }

                show.IsActive = !show.IsActive;
                show.UpdatedAt = DateTime.Now;
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string status = show.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"✅ Show '{show.Title}' {status} successfully!",
                        isActive = show.IsActive
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

        // =========================
        // GET: Admin/Shows/Details/5
        // =========================
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var show = await _context.Shows
                .Include(s => s.GenreNavigation)
                .Include(s => s.Episodes.OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber))
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
            {
                TempData["Error"] = $"❌ Show with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Details.cshtml", show);
        }
    }
}