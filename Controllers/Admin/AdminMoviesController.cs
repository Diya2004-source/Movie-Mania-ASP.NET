using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminMoviesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: AdminMovies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return View("~/Views/Admin/Movies/Index.cshtml", movies);
        }

        // GET: AdminMovies/Create
        public IActionResult Create()
        {
            return View("~/Views/Admin/Movies/Create.cshtml");
        }

        // POST: AdminMovies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile? ThumbnailFile)
        {
            try
            {
                // Check if model state is valid
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"Validation failed: <br/>{errorMessage}";
                    return View("~/Views/Admin/Movies/Create.cshtml", movie);
                }

                // Validate required fields
                if (string.IsNullOrEmpty(movie.Title))
                {
                    TempData["Error"] = "Movie title is required!";
                    return View("~/Views/Admin/Movies/Create.cshtml", movie);
                }

                // Handle file upload if provided
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    try
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "movies");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ThumbnailFile.CopyToAsync(fileStream);
                        }

                        movie.ThumbnailUrl = "/uploads/movies/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error uploading file: {ex.Message}";
                        return View("~/Views/Admin/Movies/Create.cshtml", movie);
                    }
                }

                // Set default values
                movie.CreatedAt = DateTime.Now;
                movie.ViewsCount = 0;

                // Add to database
                await _context.Movies.AddAsync(movie);
                int result = await _context.SaveChangesAsync();

                // Check if save was successful
                if (result > 0)
                {
                    TempData["Success"] = $"✅ Movie '{movie.Title}' added successfully to database!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ Movie was not added to database. Please try again.";
                    return View("~/Views/Admin/Movies/Create.cshtml", movie);
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database-specific errors
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                TempData["Error"] = $"❌ Database error: {innerMessage}";
                return View("~/Views/Admin/Movies/Create.cshtml", movie);
            }
            catch (Exception ex)
            {
                // Handle any other errors
                TempData["Error"] = $"❌ Unexpected error: {ex.Message}";
                return View("~/Views/Admin/Movies/Create.cshtml", movie);
            }
        }

        // GET: AdminMovies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                TempData["Error"] = $"❌ Movie with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Movies/Edit.cshtml", movie);
        }

        // POST: AdminMovies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? ThumbnailFile)
        {
            if (id != movie.Id)
            {
                TempData["Error"] = "❌ Movie ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"Validation failed: <br/>{errorMessage}";
                    return View("~/Views/Admin/Movies/Edit.cshtml", movie);
                }

                // Get existing movie to preserve some fields
                var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                if (existingMovie == null)
                {
                    TempData["Error"] = $"❌ Movie with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Handle file upload if provided
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    try
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "movies");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ThumbnailFile.CopyToAsync(fileStream);
                        }

                        movie.ThumbnailUrl = "/uploads/movies/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error uploading file: {ex.Message}";
                        return View("~/Views/Admin/Movies/Edit.cshtml", movie);
                    }
                }

                movie.UpdatedAt = DateTime.Now;
                movie.CreatedAt = existingMovie.CreatedAt; // Preserve original creation date
                movie.ViewsCount = existingMovie.ViewsCount; // Preserve view count

                _context.Update(movie);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Movie '{movie.Title}' updated successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ No changes were saved to the database.";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Movies.Any(m => m.Id == movie.Id))
                {
                    TempData["Error"] = $"❌ Movie with ID {movie.Id} no longer exists.";
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
                return View("~/Views/Admin/Movies/Edit.cshtml", movie);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AdminMovies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                TempData["Error"] = $"❌ Movie with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Movies/Delete.cshtml", movie);
        }

        // POST: AdminMovies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    TempData["Error"] = $"❌ Movie with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Movies.Remove(movie);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Movie '{movie.Title}' deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ Movie was not deleted from database.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error deleting movie: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AdminMovies/ToggleStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    TempData["Error"] = $"❌ Movie with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                movie.IsActive = !movie.IsActive;
                movie.UpdatedAt = DateTime.Now;

                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string status = movie.IsActive ? "activated" : "deactivated";
                    TempData["Success"] = $"✅ Movie '{movie.Title}' {status} successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ Status change failed.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}