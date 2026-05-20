using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Security.Claims;
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
using System.Linq;
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
        // GET: /User/Home
=======
<<<<<<< HEAD
        // GET: /User/Home
=======
        //Code to GET: /User/Home
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Auth", new { area = "Guest" });

            var userId = int.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(userId);

            ViewBag.UserName = user?.Name ?? "User";

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
            // Get movies for dashboard
            ViewBag.Movies = await _context.Movies
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .Take(10)
                .ToListAsync();

            // Get TV shows for dashboard
            ViewBag.Shows = await _context.Shows
                .Include(s => s.Episodes)
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .Take(10)
                .ToListAsync();

            // Get anime for dashboard
            ViewBag.Anime = await _context.Movies
                .Where(m => m.IsActive && m.Genre != null && m.Genre.ToLower() == "anime")
                .OrderByDescending(m => m.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View("~/Views/User/Home/Index.cshtml");
        }

        // GET: /User/Home/Movies
        public async Task<IActionResult> Movies(string? genre, string? sortBy, int page = 1)
        {
            var query = _context.Movies.Where(m => m.IsActive);

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(m => m.Genre != null && m.Genre == genre);
            }

            int totalCount = await query.CountAsync();

            query = sortBy switch
            {
                "rating" => query.OrderByDescending(m => m.Rating),
                "latest" => query.OrderByDescending(m => m.CreatedAt),
                "views" => query.OrderByDescending(m => m.ViewsCount),
                _ => query.OrderByDescending(m => m.CreatedAt)
            };

            var movies = await query
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();

            var genres = await _context.Movies
                .Where(m => m.IsActive && m.Genre != null)
                .Select(m => m.Genre!)
                .Distinct()
                .ToListAsync();

            ViewBag.Genres = genres;
            ViewBag.CurrentGenre = genre ?? "All";
            ViewBag.CurrentSort = sortBy ?? "latest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / 20);
            ViewBag.TotalItems = totalCount;

            return View("~/Views/User/Home/Movies.cshtml", movies);
        }

        // GET: /User/Home/Shows
        public async Task<IActionResult> Shows(string? genre, string? sortBy, int page = 1)
        {
            var query = _context.Shows
                .Include(s => s.Episodes)
                .Where(s => s.IsActive);

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(s => s.Genre != null && s.Genre == genre);
            }

            int totalCount = await query.CountAsync();

            query = sortBy switch
            {
                "rating" => query.OrderByDescending(s => s.Rating),
                "latest" => query.OrderByDescending(s => s.CreatedAt),
                "views" => query.OrderByDescending(s => s.ViewsCount),
                _ => query.OrderByDescending(s => s.CreatedAt)
            };

            var shows = await query
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();

            var genres = await _context.Shows
                .Where(s => s.IsActive && s.Genre != null)
                .Select(s => s.Genre!)
                .Distinct()
                .ToListAsync();

            ViewBag.Genres = genres;
            ViewBag.CurrentGenre = genre ?? "All";
            ViewBag.CurrentSort = sortBy ?? "latest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / 20);
            ViewBag.TotalItems = totalCount;

            return View("~/Views/User/Home/Shows.cshtml", shows);
        }

        // GET: /User/Home/Anime
        public async Task<IActionResult> Anime(string? sortBy, int page = 1)
        {
            var query = _context.Movies
                .Where(m => m.IsActive && m.Genre != null && m.Genre.ToLower() == "anime");

            int totalCount = await query.CountAsync();

            query = sortBy switch
            {
                "rating" => query.OrderByDescending(m => m.Rating),
                "latest" => query.OrderByDescending(m => m.CreatedAt),
                "views" => query.OrderByDescending(m => m.ViewsCount),
                _ => query.OrderByDescending(m => m.CreatedAt)
            };

            var animes = await query
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();

            ViewBag.CurrentSort = sortBy ?? "latest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / 20);
            ViewBag.TotalItems = totalCount;

            return View("~/Views/User/Home/Anime.cshtml", animes);
        }

        // GET: /User/Home/MovieDetails/5
        public async Task<IActionResult> MovieDetails(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
                return NotFound();

            return View("~/Views/User/Home/MovieDetails.cshtml", movie);
        }

        // GET: /User/Home/ShowDetails/5
        public async Task<IActionResult> ShowDetails(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (show == null)
                return NotFound();

            ViewBag.TotalSeasons = show.Episodes?.Select(e => e.SeasonNumber).Distinct().Count() ?? 0;
            ViewBag.TotalEpisodes = show.Episodes?.Count ?? 0;
            ViewBag.EpisodesBySeason = show.Episodes?
                .GroupBy(e => e.SeasonNumber)
                .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<int, List<Episode>>();

            return View("~/Views/User/Home/ShowDetails.cshtml", show);
        }
    }
<<<<<<< HEAD
=======
=======
            // Build a dashboard view model rather than using ViewBag
            var dashboard = new DashboardViewModel
            {
                Movies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(10)
                    .ToListAsync(),

                Shows = await _context.Shows
                    .Include(s => s.Episodes)
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(10)
                    .ToListAsync(),

                Anime = await _context.Movies
                    .Where(m => m.IsActive && m.Genre != null && m.Genre.ToLower() == "anime")
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(10)
                    .ToListAsync()
            };

            return View("~/Views/User/Home/Index.cshtml", dashboard);
        }
    }

    // ViewModel for dashboard
    public class DashboardViewModel
    {
        public List<Movie> Movies { get; set; } = new();
        public List<Show> Shows { get; set; } = new();
        public List<Movie> Anime { get; set; } = new();
    }
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
}