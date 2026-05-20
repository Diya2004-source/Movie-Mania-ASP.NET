using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Movies")]
    public class AdminMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string _viewPath = "~/Views/Admin/Movies/";

        public AdminMoviesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return View(_viewPath + "Index.cshtml", movies);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(_viewPath + "Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile? thumbnailFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View(_viewPath + "Create.cshtml", movie);
                }

                movie.ThumbnailUrl = await UploadFile(thumbnailFile, "movies");
                movie.CreatedAt = DateTime.Now;
                movie.ViewsCount = 0;

                await _context.Movies.AddAsync(movie);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"Movie '{movie.Title}' added successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Movie was not added to database.";
                return View(_viewPath + "Create.cshtml", movie);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(_viewPath + "Create.cshtml", movie);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                TempData["Error"] = $"Movie with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(_viewPath + "Edit.cshtml", movie);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? thumbnailFile)
        {
            if (id != movie.Id)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("<br/>", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Validation failed:<br/>{errors}";
                    return View(_viewPath + "Edit.cshtml", movie);
                }

                var existingMovie = await _context.Movies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (existingMovie == null)
                {
                    TempData["Error"] = $"Movie with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (thumbnailFile != null)
                    movie.ThumbnailUrl = await UploadFile(thumbnailFile, "movies");

                movie.CreatedAt = existingMovie.CreatedAt;
                movie.ViewsCount = existingMovie.ViewsCount;
                movie.UpdatedAt = DateTime.Now;

                _context.Update(movie);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"Movie '{movie.Title}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No changes were saved.";
                return View(_viewPath + "Edit.cshtml", movie);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Movies.AnyAsync(m => m.Id == movie.Id))
                {
                    TempData["Error"] = $"Movie with ID {movie.Id} no longer exists.";
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(_viewPath + "Edit.cshtml", movie);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                TempData["Error"] = $"Movie with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Delete.cshtml", movie);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.Reviews)
                    .Include(m => m.UserActivities)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    TempData["Error"] = "Movie not found.";
                    return RedirectToAction(nameof(Index));
                }

                // 🔥 STEP 1: Delete Reviews
                if (movie.Reviews != null && movie.Reviews.Any())
                    _context.MovieReviews.RemoveRange(movie.Reviews);

                // 🔥 STEP 2: Delete User Activities
                if (movie.UserActivities != null && movie.UserActivities.Any())
                    _context.UserActivities.RemoveRange(movie.UserActivities);

                // 🔥 STEP 3: DELETE WISHLIST (correct way — NO INCLUDE)
                var wishlists = _context.Wishlists
                    .Where(w => w.MovieId == id)
                    .ToList();

                if (wishlists.Any())
                    _context.Wishlists.RemoveRange(wishlists);

                // 🔥 STEP 4: Delete Movie
                _context.Movies.Remove(movie);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Movie deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting movie: " +
                    (ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                    return Json(new { success = false, message = "Movie not found" });

                movie.IsActive = !movie.IsActive;
                movie.UpdatedAt = DateTime.Now;

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var status = movie.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"Movie '{movie.Title}' {status} successfully!",
                        isActive = movie.IsActive
                    });
                }

                return Json(new { success = false, message = "No changes were saved." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                TempData["Error"] = $"Movie with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Details.cshtml", movie);
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