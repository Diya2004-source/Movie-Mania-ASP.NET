<<<<<<< HEAD
﻿//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MovieMania.Models;

//namespace MovieMania.Controllers.Admin
//{
//    [Authorize(Roles = "admin")]
//    [Route("Admin/Anime")]
//    public class AdminAnimeController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IWebHostEnvironment _env;
//        private readonly string _viewPath = "~/Views/Admin/Anime/";

//        public AdminAnimeController(ApplicationDbContext context, IWebHostEnvironment env)
//        {
//            _context = context;
//            _env = env;
//        }

//        [HttpGet("")]
//        public async Task<IActionResult> Index()
//        {
//            var animes = await _context.Shows.Where(s => s.Genre == "Anime").ToListAsync();
//            return View(_viewPath + "Index.cshtml", animes);
//        }

//        [HttpGet("Details/{id}")]
//        public async Task<IActionResult> Details(int id)
//        {
//            var anime = await _context.Shows.Include(s => s.Episodes).FirstOrDefaultAsync(m => m.Id == id);
//            if (anime == null) return NotFound();
//            return View(_viewPath + "Details.cshtml", anime);
//        }

//        [HttpGet("Create")]
//        public IActionResult Create() => View(_viewPath + "Create.cshtml");

//        [HttpPost("Create")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Show anime, IFormFile? ThumbnailFile)
//        {
//            anime.Genre = "Anime";
//            anime.CreatedAt = DateTime.Now;
//            ModelState.Remove("Genre");
//            ModelState.Remove("ThumbnailUrl");
//            if (ModelState.IsValid)
//            {
//                if (ThumbnailFile != null) anime.ThumbnailUrl = await SaveImage(ThumbnailFile);
//                _context.Add(anime);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(_viewPath + "Create.cshtml", anime);
//        }

//        [HttpGet("Edit/{id}")]
//        public async Task<IActionResult> Edit(int id)
//        {
//            var anime = await _context.Shows.FindAsync(id);
//            if (anime == null) return NotFound();
//            return View(_viewPath + "Edit.cshtml", anime);
//        }

//        [HttpPost("Edit/{id}")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Show anime, IFormFile? ThumbnailFile)
//        {
//            if (id != anime.Id) return BadRequest();
//            anime.Genre = "Anime";
//            ModelState.Remove("Genre");
//            ModelState.Remove("ThumbnailUrl");
//            if (ModelState.IsValid)
//            {
//                if (ThumbnailFile != null) anime.ThumbnailUrl = await SaveImage(ThumbnailFile);
//                _context.Update(anime);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(_viewPath + "Edit.cshtml", anime);
//        }

//        [HttpGet("AddEpisode/{id}")]
//        public async Task<IActionResult> AddEpisode(int id)
//        {
//            var anime = await _context.Shows.FindAsync(id);
//            if (anime == null) return NotFound();
//            ViewBag.AnimeTitle = anime.Title;
//            return View(_viewPath + "AddEpisode.cshtml", new Episode { ShowId = id });
//        }

//        [HttpPost("AddEpisode/{id}")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> AddEpisode(int id, Episode episode)
//        {
//            episode.ShowId = id;
//            ModelState.Remove("Show");
//            if (ModelState.IsValid)
//            {
//                _context.Episodes.Add(episode);
//                await _context.SaveChangesAsync();
//                return RedirectToAction("Details", new { id = id });
//            }
//            return View(_viewPath + "AddEpisode.cshtml", episode);
//        }

//        private async Task<string> SaveImage(IFormFile file)
//        {
//            string folder = Path.Combine(_env.WebRootPath, "images", "movies");
//            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
//            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
//            using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create)) { await file.CopyToAsync(stream); }
//            return "/images/movies/" + fileName;
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
=======
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Anime")]
    public class AdminAnimeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
<<<<<<< HEAD
        private readonly string _viewPath = "~/Views/Admin/Anime/";
=======
        private readonly string viewFolder = "~/Views/Admin/Anime/";
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d

        public AdminAnimeController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

<<<<<<< HEAD
        // ================= INDEX =================

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var animes = await _context.Shows
                .Where(s => s.Genre == "Anime")
                .ToListAsync();

            return View(_viewPath + "Index.cshtml", animes);
        }

        // ================= DETAILS =================

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var anime = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (anime == null)
                return NotFound();

            return View(_viewPath + "Details.cshtml", anime);
        }

        // ================= CREATE =================

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(_viewPath + "Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Show anime, IFormFile? ThumbnailFile)
        {
            anime.Genre = "Anime";
            anime.CreatedAt = DateTime.Now;

            ModelState.Remove("Genre");
            ModelState.Remove("ThumbnailUrl");
            ModelState.Remove("Episodes");

            if (ModelState.IsValid)
            {
                if (ThumbnailFile != null)
                {
                    anime.ThumbnailUrl = await SaveImage(ThumbnailFile);
                }

                _context.Shows.Add(anime);
                await _context.SaveChangesAsync();

                TempData["success"] = "Anime created successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Create.cshtml", anime);
        }

        // ================= EDIT =================

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var anime = await _context.Shows.FindAsync(id);

            if (anime == null)
                return NotFound();

            return View(_viewPath + "Edit.cshtml", anime);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Show anime, IFormFile? ThumbnailFile)
        {
            if (id != anime.Id)
                return BadRequest();

            var existingAnime = await _context.Shows.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingAnime == null)
                return NotFound();

            anime.Genre = "Anime";

            ModelState.Remove("Genre");
            ModelState.Remove("ThumbnailUrl");
            ModelState.Remove("Episodes");

            if (ModelState.IsValid)
            {
                if (ThumbnailFile != null)
                {
                    anime.ThumbnailUrl = await SaveImage(ThumbnailFile);
                }
                else
                {
                    anime.ThumbnailUrl = existingAnime.ThumbnailUrl;
                }

                anime.CreatedAt = existingAnime.CreatedAt;

                _context.Shows.Update(anime);

                await _context.SaveChangesAsync();

                TempData["success"] = "Anime updated successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(_viewPath + "Edit.cshtml", anime);
        }

        // ================= DELETE =================

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var anime = await _context.Shows
                .FirstOrDefaultAsync(x => x.Id == id);

            if (anime == null)
                return NotFound();

            return View(_viewPath + "Delete.cshtml", anime);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anime = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (anime == null)
                return NotFound();

            // delete episodes first
            if (anime.Episodes != null && anime.Episodes.Any())
            {
                _context.Episodes.RemoveRange(anime.Episodes);
            }

            // delete image
            if (!string.IsNullOrEmpty(anime.ThumbnailUrl))
            {
                string imagePath = Path.Combine(
                    _env.WebRootPath,
                    anime.ThumbnailUrl.TrimStart('/')
                        .Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Shows.Remove(anime);

            await _context.SaveChangesAsync();

            TempData["success"] = "Anime deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        // ================= ADD EPISODE =================

        [HttpGet("AddEpisode/{id}")]
        public async Task<IActionResult> AddEpisode(int id)
        {
            var anime = await _context.Shows.FindAsync(id);

            if (anime == null)
                return NotFound();

            ViewBag.AnimeTitle = anime.Title;

            return View(_viewPath + "AddEpisode.cshtml",
                new Episode
                {
                    ShowId = id
                });
        }

        [HttpPost("AddEpisode/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEpisode(int id, Episode episode)
        {
            try
            {
                // force new record
                episode.Id = 0;

                episode.ShowId = id;

                episode.CreatedAt = DateTime.Now;

                // remove navigation validation
                ModelState.Remove("Show");

                if (ModelState.IsValid)
                {
                    await _context.Episodes.AddAsync(episode);

                    await _context.SaveChangesAsync();

                    TempData["success"] = "Episode added successfully";

                    return RedirectToAction("Details", new { id });
=======
        // GET: Admin/Anime
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // Log authentication status for debugging
            Console.WriteLine($"===== AdminAnime Index Access =====");
            Console.WriteLine($"User: {User.Identity?.Name ?? "No Name"}");
            Console.WriteLine($"Authenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"IsAdmin: {User.IsInRole("admin")}");

            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                Console.WriteLine("❌ Not authenticated as admin, redirecting to login");
                return RedirectToAction("Login", "Admin", new { area = "" });
            }

            Console.WriteLine("✅ Authenticated as admin, loading anime...");

            var animes = await _context.Shows
                .Where(s => s.Genre == "Anime")
                .Include(s => s.Episodes)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(viewFolder + "Index.cshtml", animes);
        }

        // GET: Admin/Anime/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Admin", new { area = "" });
            }
            return View(viewFolder + "Create.cshtml");
        }

        // POST: Admin/Anime/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Show anime, IFormFile? ThumbnailFile, IFormFile? PosterFile)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Admin", new { area = "" });
            }

            try
            {
                anime.Genre = "Anime";

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"❌ Validation failed: <br/>{errorMessage}";
                    return View(viewFolder + "Create.cshtml", anime);
                }

                // Handle thumbnail upload
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "anime", "thumbnails");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }

                    anime.ThumbnailUrl = "/uploads/anime/thumbnails/" + uniqueFileName;
                }

                // Handle poster upload
                if (PosterFile != null && PosterFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "anime", "posters");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + PosterFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterFile.CopyToAsync(fileStream);
                    }

                    anime.PosterUrl = "/uploads/anime/posters/" + uniqueFileName;
                }

                anime.CreatedAt = DateTime.Now;
                anime.ViewsCount = 0;

                await _context.Shows.AddAsync(anime);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Anime '{anime.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ Anime was not created. Please try again.";
                    return View(viewFolder + "Create.cshtml", anime);
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                }
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                ModelState.AddModelError("", ex.InnerException?.Message ?? ex.Message);
            }

            var anime = await _context.Shows.FindAsync(id);

            if (anime != null)
            {
                ViewBag.AnimeTitle = anime.Title;
            }

            return View(_viewPath + "AddEpisode.cshtml", episode);
        }        // ================= SAVE IMAGE =================

        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_env.WebRootPath, "images", "movies");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/movies/" + fileName;
=======
                TempData["Error"] = $"❌ Error: {ex.Message}";
                return View(viewFolder + "Create.cshtml", anime);
            }
        }

        // GET: Admin/Anime/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Admin", new { area = "" });
            }

            var anime = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id && s.Genre == "Anime");

            if (anime == null)
            {
                TempData["Error"] = $"❌ Anime with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Edit.cshtml", anime);
        }

        // POST: Admin/Anime/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Show anime, IFormFile? ThumbnailFile, IFormFile? PosterFile)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Admin", new { area = "" });
            }

            if (id != anime.Id)
            {
                return NotFound();
            }

            try
            {
                anime.Genre = "Anime";

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    string errorMessage = string.Join("<br/>", errors);
                    TempData["Error"] = $"❌ Validation failed: <br/>{errorMessage}";
                    return View(viewFolder + "Edit.cshtml", anime);
                }

                var existingAnime = await _context.Shows.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                if (existingAnime == null)
                {
                    TempData["Error"] = $"❌ Anime with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Handle thumbnail upload
                if (ThumbnailFile != null && ThumbnailFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "anime", "thumbnails");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ThumbnailFile.CopyToAsync(fileStream);
                    }

                    anime.ThumbnailUrl = "/uploads/anime/thumbnails/" + uniqueFileName;
                }

                // Handle poster upload
                if (PosterFile != null && PosterFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "anime", "posters");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + PosterFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await PosterFile.CopyToAsync(fileStream);
                    }

                    anime.PosterUrl = "/uploads/anime/posters/" + uniqueFileName;
                }

                anime.CreatedAt = existingAnime.CreatedAt;
                anime.ViewsCount = existingAnime.ViewsCount;
                anime.UpdatedAt = DateTime.Now;

                _context.Shows.Update(anime);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Anime '{anime.Title}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "❌ No changes were saved.";
                    return View(viewFolder + "Edit.cshtml", anime);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error: {ex.Message}";
                return View(viewFolder + "Edit.cshtml", anime);
            }
        }

        // GET: Admin/Anime/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Admin", new { area = "" });
            }

            var anime = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id && s.Genre == "Anime");

            if (anime == null)
            {
                TempData["Error"] = $"❌ Anime with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Delete.cshtml", anime);
        }

        // POST: Admin/Anime/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Admin", new { area = "" });
            }

            try
            {
                var anime = await _context.Shows
                    .Include(s => s.Episodes)
                    .FirstOrDefaultAsync(s => s.Id == id && s.Genre == "Anime");

                if (anime == null)
                {
                    TempData["Error"] = $"❌ Anime with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (anime.Episodes != null && anime.Episodes.Any())
                {
                    _context.Episodes.RemoveRange(anime.Episodes);
                }

                _context.Shows.Remove(anime);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["Success"] = $"✅ Anime '{anime.Title}' deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "❌ Anime was not deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error deleting anime: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Anime/ToggleStatus/5
        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return Json(new { success = false, message = "Not authenticated", redirect = "/Admin/Login" });
            }

            try
            {
                var anime = await _context.Shows.FindAsync(id);
                if (anime == null)
                {
                    return Json(new { success = false, message = "Anime not found" });
                }

                anime.IsActive = !anime.IsActive;
                anime.UpdatedAt = DateTime.Now;
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    string status = anime.IsActive ? "activated" : "deactivated";
                    return Json(new
                    {
                        success = true,
                        message = $"✅ Anime '{anime.Title}' {status} successfully!",
                        isActive = anime.IsActive
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

        // GET: Admin/Anime/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
            {
                return RedirectToAction("Login", "Admin", new { area = "" });
            }

            var anime = await _context.Shows
                .Include(s => s.Episodes.OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber))
                .FirstOrDefaultAsync(s => s.Id == id && s.Genre == "Anime");

            if (anime == null)
            {
                TempData["Error"] = $"❌ Anime with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewFolder + "Details.cshtml", anime);
        }

        // Debug methods
        [HttpGet("Debug")]
        public IActionResult Debug()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var isAdmin = User.IsInRole("admin");
            var userName = User.Identity?.Name ?? "Not logged in";

            string result = $"<h2>AdminAnimeController Debug</h2>";
            result += $"<p><strong>Authenticated:</strong> {isAuthenticated}</p>";
            result += $"<p><strong>Is Admin:</strong> {isAdmin}</p>";
            result += $"<p><strong>User Name:</strong> {userName}</p>";

            if (isAuthenticated)
            {
                result += "<h3>Claims:</h3><ul>";
                foreach (var claim in User.Claims)
                {
                    result += $"<li><strong>{claim.Type}:</strong> {claim.Value}</li>";
                }
                result += "</ul>";
            }

            result += "<p><a href='/Admin/Login'>Go to Admin Login</a></p>";
            result += "<p><a href='/Admin/Anime'>Go to Anime Page</a></p>";
            result += "<p><a href='/Admin/Anime/TestAuth'>Test Authentication</a></p>";

            return Content(result, "text/html");
        }

        [HttpGet("TestAuth")]
        public IActionResult TestAuth()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var isAdmin = User.IsInRole("admin");
            var userName = User.Identity?.Name ?? "Not logged in";

            if (isAuthenticated && isAdmin)
            {
                return Content($"✅ SUCCESS! You ARE authenticated as admin.<br>User: {userName}<br><a href='/Admin/Anime'>Go to Anime Page</a>", "text/html");
            }
            else if (isAuthenticated && !isAdmin)
            {
                return Content($"⚠️ You are logged in but NOT an admin.<br>User: {userName}<br>Role: Not Admin<br><a href='/Admin/Login'>Login as admin</a>", "text/html");
            }
            else
            {
                return Content($"❌ You are NOT authenticated.<br><a href='/Admin/Login'>Login as admin</a>", "text/html");
            }
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
        }
    }
}