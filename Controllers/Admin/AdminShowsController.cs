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
        private readonly string _viewPath = "~/Views/Admin/Shows/";

        public AdminShowsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var shows = await _context.Shows
                .Include(s => s.Episodes)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(_viewPath + "Index.cshtml", shows);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(_viewPath + "Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Show show, IFormFile? thumbnailFile, IFormFile? posterFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View(_viewPath + "Create.cshtml", show);
                }

                show.ThumbnailUrl = await UploadFile(thumbnailFile, "shows/thumbnails");
                show.PosterUrl = await UploadFile(posterFile, "shows/posters");
                show.CreatedAt = DateTime.Now;
                show.ViewsCount = 0;

                await _context.Shows.AddAsync(show);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"Show '{show.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Show was not created.";
                return View(_viewPath + "Create.cshtml", show);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(_viewPath + "Create.cshtml", show);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
            {
                TempData["Error"] = $"Show with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(_viewPath + "Edit.cshtml", show);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Show show, IFormFile? thumbnailFile, IFormFile? posterFile)
        {
            if (id != show.Id)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View(_viewPath + "Edit.cshtml", show);
                }

                var existingShow = await _context.Shows
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (existingShow == null)
                {
                    TempData["Error"] = $"Show with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (thumbnailFile != null)
                    show.ThumbnailUrl = await UploadFile(thumbnailFile, "shows/thumbnails");
                if (posterFile != null)
                    show.PosterUrl = await UploadFile(posterFile, "shows/posters");

                show.CreatedAt = existingShow.CreatedAt;
                show.ViewsCount = existingShow.ViewsCount;
                show.UpdatedAt = DateTime.Now;

                _context.Update(show);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"Show '{show.Title}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No changes were saved.";
                return View(_viewPath + "Edit.cshtml", show);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(_viewPath + "Edit.cshtml", show);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
            {
                TempData["Error"] = $"Show with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(_viewPath + "Delete.cshtml", show);
        }

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
                    TempData["Error"] = $"Show with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (show.Episodes != null && show.Episodes.Any())
                    _context.Episodes.RemoveRange(show.Episodes);

                _context.Shows.Remove(show);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    TempData["Success"] = $"Show '{show.Title}' deleted successfully!";
                else
                    TempData["Error"] = "Show was not deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting show: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Episodes.OrderBy(e => e.SeasonNumber))
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
            {
                TempData["Error"] = $"Show with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Details.cshtml", show);
        }

        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var show = await _context.Shows.FindAsync(id);
                if (show == null)
                    return Json(new { success = false, message = "Show not found" });

                show.IsActive = !show.IsActive;
                show.UpdatedAt = DateTime.Now;

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var status = show.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"Show '{show.Title}' {status} successfully!",
                        isActive = show.IsActive
                    });
                }

                return Json(new { success = false, message = "No changes were saved." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private async Task<string?> UploadFile(IFormFile? file, string subFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return $"/uploads/{subFolder}/{uniqueFileName}";
        }
    }
}