using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Episodes")]
    public class AdminEpisodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string viewFolder = "~/Views/Admin/Episodes/";

        public AdminEpisodesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =========================
        // GET: Admin/Episodes/Show/5
        // =========================
        [HttpGet("Show/{showId}")]
        public async Task<IActionResult> Index(int showId)
        {
            var show = await _context.Shows.FindAsync(showId);
            if (show == null)
            {
                TempData["Error"] = $"❌ Show with ID {showId} not found.";
                return RedirectToAction("Index", "AdminShows");
            }

            var episodes = await _context.Episodes
                .Where(e => e.ShowId == showId)
                .OrderBy(e => e.SeasonNumber)
                .ThenBy(e => e.EpisodeNumber)
                .ToListAsync();

            ViewBag.ShowId = showId;
            ViewBag.ShowTitle = show.Title;
            ViewBag.TotalSeasons = show.TotalSeasons;
            ViewBag.TotalEpisodes = show.TotalEpisodes;

            return View(viewFolder + "Index.cshtml", episodes);
        }

        // =========================
        // GET: Admin/Episodes/Create/5
        // =========================
        [HttpGet("Create/{showId}")]
        public async Task<IActionResult> Create(int showId)
        {
            var show = await _context.Shows.FindAsync(showId);
            if (show == null)
            {
                TempData["Error"] = $"❌ Show with ID {showId} not found.";
                return RedirectToAction("Index", "AdminShows");
            }

            ViewBag.ShowId = showId;
            ViewBag.ShowTitle = show.Title;
            return View(viewFolder + "Create.cshtml");
        }

        // =========================
        // POST: Admin/Episodes/Create
        // =========================
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Episode episode, IFormFile? ThumbnailFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"❌ Validation failed: <br/>{errorMessage}";

                    var show = await _context.Shows.FindAsync(episode.ShowId);
                    ViewBag.ShowId = episode.ShowId;
                    ViewBag.ShowTitle = show?.Title ?? "Unknown";
                    return View(viewFolder + "Create.cshtml", episode);
                }

                // Check if episode number already exists
                var existingEpisode = await _context.Episodes
                    .FirstOrDefaultAsync(e => e.ShowId == episode.ShowId &&
                                              e.SeasonNumber == episode.SeasonNumber &&
                                              e.EpisodeNumber == episode.EpisodeNumber);

                if (existingEpisode != null)
                {
                    TempData["Error"] = $"❌ Episode S{episode.SeasonNumber}E{episode.EpisodeNumber} already exists for this show.";

                    var show = await _context.Shows.FindAsync(episode.ShowId);
                    ViewBag.ShowId = episode.ShowId;
                    ViewBag.ShowTitle = show?.Title ?? "Unknown";
                    return View(viewFolder + "Create.cshtml", episode);
                }

                // Handle thumbnail upload
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "episodes");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }

                    episode.ThumbnailUrl = "/uploads/episodes/" + uniqueFileName;
                }

                episode.CreatedAt = DateTime.Now;
                episode.ViewsCount = 0;

                await _context.Episodes.AddAsync(episode);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    // Update show's total episodes count
                    var show = await _context.Shows.FindAsync(episode.ShowId);
                    if (show != null)
                    {
                        show.TotalEpisodes = await _context.Episodes.CountAsync(e => e.ShowId == episode.ShowId);
                        show.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = $"✅ Episode S{episode.SeasonNumber}E{episode.EpisodeNumber} - '{episode.Title}' created successfully!";
                    return RedirectToAction(nameof(Index), new { showId = episode.ShowId });
                }
                else
                {
                    TempData["Error"] = "❌ Episode was not created. Please try again.";

                    var show = await _context.Shows.FindAsync(episode.ShowId);
                    ViewBag.ShowId = episode.ShowId;
                    ViewBag.ShowTitle = show?.Title ?? "Unknown";
                    return View(viewFolder + "Create.cshtml", episode);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";

                var show = await _context.Shows.FindAsync(episode.ShowId);
                ViewBag.ShowId = episode.ShowId;
                ViewBag.ShowTitle = show?.Title ?? "Unknown";
                return View(viewFolder + "Create.cshtml", episode);
            }
        }

        // =========================
        // GET: Admin/Episodes/Edit/5
        // =========================
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var episode = await _context.Episodes
                .Include(e => e.Show)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (episode == null)
            {
                TempData["Error"] = $"❌ Episode with ID {id} not found.";
                return RedirectToAction("Index", "AdminShows");
            }

            ViewBag.ShowId = episode.ShowId;
            ViewBag.ShowTitle = episode.Show?.Title;
            return View(viewFolder + "Edit.cshtml", episode);
        }

        // =========================
        // POST: Admin/Episodes/Edit/5
        // =========================
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Episode episode, IFormFile? ThumbnailFile)
        {
            if (id != episode.Id)
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

                    var show = await _context.Shows.FindAsync(episode.ShowId);
                    ViewBag.ShowId = episode.ShowId;
                    ViewBag.ShowTitle = show?.Title ?? "Unknown";
                    return View(viewFolder + "Edit.cshtml", episode);
                }

                var existingEpisode = await _context.Episodes.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                if (existingEpisode == null)
                {
                    TempData["Error"] = $"❌ Episode with ID {id} not found.";
                    return RedirectToAction("Index", "AdminShows");
                }

                // Check if another episode with same season/episode number exists (excluding current)
                var duplicateEpisode = await _context.Episodes
                    .FirstOrDefaultAsync(e => e.ShowId == episode.ShowId &&
                                              e.SeasonNumber == episode.SeasonNumber &&
                                              e.EpisodeNumber == episode.EpisodeNumber &&
                                              e.Id != id);

                if (duplicateEpisode != null)
                {
                    TempData["Error"] = $"❌ Episode S{episode.SeasonNumber}E{episode.EpisodeNumber} already exists for this show.";

                    var show = await _context.Shows.FindAsync(episode.ShowId);
                    ViewBag.ShowId = episode.ShowId;
                    ViewBag.ShowTitle = show?.Title ?? "Unknown";
                    return View(viewFolder + "Edit.cshtml", episode);
                }

                // Handle thumbnail upload
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "episodes");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }

                    episode.ThumbnailUrl = "/uploads/episodes/" + uniqueFileName;
                }

                episode.CreatedAt = existingEpisode.CreatedAt;
                episode.ViewsCount = existingEpisode.ViewsCount;
                episode.UpdatedAt = DateTime.Now;

                _context.Episodes.Update(episode);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Episode S{episode.SeasonNumber}E{episode.EpisodeNumber} - '{episode.Title}' updated successfully!";
                    return RedirectToAction(nameof(Index), new { showId = episode.ShowId });
                }
                else
                {
                    TempData["Error"] = "❌ No changes were saved.";

                    var show = await _context.Shows.FindAsync(episode.ShowId);
                    ViewBag.ShowId = episode.ShowId;
                    ViewBag.ShowTitle = show?.Title ?? "Unknown";
                    return View(viewFolder + "Edit.cshtml", episode);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";

                var show = await _context.Shows.FindAsync(episode.ShowId);
                ViewBag.ShowId = episode.ShowId;
                ViewBag.ShowTitle = show?.Title ?? "Unknown";
                return View(viewFolder + "Edit.cshtml", episode);
            }
        }

        // =========================
        // GET: Admin/Episodes/Delete/5
        // =========================
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var episode = await _context.Episodes
                .Include(e => e.Show)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (episode == null)
            {
                TempData["Error"] = $"❌ Episode with ID {id} not found.";
                return RedirectToAction("Index", "AdminShows");
            }

            return View(viewFolder + "Delete.cshtml", episode);
        }

        // =========================
        // POST: Admin/Episodes/Delete/5
        // =========================
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var episode = await _context.Episodes
                    .Include(e => e.Show)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (episode == null)
                {
                    TempData["Error"] = $"❌ Episode with ID {id} not found.";
                    return RedirectToAction("Index", "AdminShows");
                }

                int showId = episode.ShowId;
                string episodeInfo = $"S{episode.SeasonNumber}E{episode.EpisodeNumber} - '{episode.Title}'";

                _context.Episodes.Remove(episode);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    // Update show's total episodes count
                    var show = await _context.Shows.FindAsync(showId);
                    if (show != null)
                    {
                        show.TotalEpisodes = await _context.Episodes.CountAsync(e => e.ShowId == showId);
                        show.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = $"✅ Episode {episodeInfo} deleted successfully!";
                    return RedirectToAction(nameof(Index), new { showId });
                }
                else
                {
                    TempData["Error"] = "❌ Episode was not deleted.";
                    return RedirectToAction(nameof(Index), new { showId = episode.ShowId });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error deleting episode: {ex.Message}";
                return RedirectToAction("Index", "AdminShows");
            }
        }

        // =========================
        // POST: Admin/Episodes/ToggleStatus/5
        // =========================
        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var episode = await _context.Episodes.FindAsync(id);
                if (episode == null)
                {
                    return Json(new { success = false, message = "Episode not found" });
                }

                episode.IsActive = !episode.IsActive;
                episode.UpdatedAt = DateTime.Now;
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string status = episode.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"✅ Episode S{episode.SeasonNumber}E{episode.EpisodeNumber} {status} successfully!",
                        isActive = episode.IsActive
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