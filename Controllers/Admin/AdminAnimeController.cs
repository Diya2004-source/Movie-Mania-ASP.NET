//using Microsoft.AspNetCore.Authorization;
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

namespace MovieMania.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Anime")]
    public class AdminAnimeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string _viewPath = "~/Views/Admin/Anime/";

        public AdminAnimeController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

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
                }
            }
            catch (Exception ex)
            {
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
        }
    }
}