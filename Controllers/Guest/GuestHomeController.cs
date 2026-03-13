using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace MovieMania.Controllers.Guest
{
    public class GuestHomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestHomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Guest/Home/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch featured movies
                var featuredMovies = await _context.Movies
                    .Where(m => m.IsActive && m.IsFeatured)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                // Fetch latest movies
                var latestMovies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(12)
                    .ToListAsync();

                // Fetch popular movies (by rating)
                var popularMovies = await _context.Movies
                    .Where(m => m.IsActive && m.Rating >= 7)
                    .OrderByDescending(m => m.Rating)
                    .Take(8)
                    .ToListAsync();

                // Fetch trending shows (by views)
                var trendingShows = await _context.Shows
                    .Include(s => s.Episodes)
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.ViewsCount)
                    .Take(6)
                    .ToListAsync();

                // Fetch all active genres
                var genres = await _context.Genres
                    .Where(g => g.IsActive)
                    .OrderBy(g => g.Name)
                    .ToListAsync();

                var viewModel = new GuestHomeViewModel
                {
                    FeaturedMovies = featuredMovies,
                    LatestMovies = latestMovies,
                    PopularMovies = popularMovies,
                    TrendingShows = trendingShows,
                    Genres = genres
                };

                return View("~/Views/Guest/Home/Index.cshtml", viewModel);
            }
            catch (System.Exception ex)
            {
                // Log error
                Console.WriteLine($"Error in GuestHomeController.Index: {ex.Message}");

                // Return empty view model on error
                var emptyViewModel = new GuestHomeViewModel
                {
                    FeaturedMovies = new System.Collections.Generic.List<Movie>(),
                    LatestMovies = new System.Collections.Generic.List<Movie>(),
                    PopularMovies = new System.Collections.Generic.List<Movie>(),
                    TrendingShows = new System.Collections.Generic.List<Show>(),
                    Genres = new System.Collections.Generic.List<Genre>()
                };

                return View("~/Views/Guest/Home/Index.cshtml", emptyViewModel);
            }
        }

        // GET: Guest/Home/Movies
        public async Task<IActionResult> Movies(string search, string genre, string sortBy = "latest", int page = 1)
        {
            int pageSize = 12;
            var query = _context.Movies.Where(m => m.IsActive);

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.Title.Contains(search) ||
                    (m.Description != null && m.Description.Contains(search)) ||
                    (m.Cast != null && m.Cast.Contains(search)) ||
                    (m.Director != null && m.Director.Contains(search)));
                ViewBag.Search = search;
            }

            // Apply genre filter
            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(m => m.Genre == genre);
                ViewBag.Genre = genre;
            }

            // Apply sorting
            query = sortBy switch
            {
                "rating" => query.OrderByDescending(m => m.Rating),
                "year" => query.OrderByDescending(m => m.ReleaseYear),
                "title" => query.OrderBy(m => m.Title),
                _ => query.OrderByDescending(m => m.CreatedAt)
            };
            ViewBag.SortBy = sortBy;

            // Get distinct genres for filter dropdown
            ViewBag.Genres = await _context.Movies
                .Where(m => m.IsActive)
                .Select(m => m.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            // Pagination
            var totalItems = await query.CountAsync();
            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Guest/Home/Movies.cshtml", movies);
        }

        // GET: Guest/Home/MovieDetails/5
        public async Task<IActionResult> MovieDetails(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.GenreNavigation)
                .Include(m => m.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
            {
                return NotFound();
            }

            // Increment view count
            movie.ViewsCount++;
            await _context.SaveChangesAsync();

            // Get related movies (same genre)
            var relatedMovies = await _context.Movies
                .Where(m => m.Genre == movie.Genre && m.Id != id && m.IsActive)
                .OrderByDescending(m => m.Rating)
                .Take(6)
                .ToListAsync();

            var viewModel = new GuestMovieDetailsViewModel
            {
                Movie = movie,
                RelatedMovies = relatedMovies,
                Reviews = movie.Reviews
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList()
            };

            return View("~/Views/Guest/Home/MovieDetails.cshtml", viewModel);
        }

        // GET: Guest/Home/Shows
        public async Task<IActionResult> Shows(string search, string genre, int page = 1)
        {
            int pageSize = 12;
            var query = _context.Shows.Where(s => s.IsActive);

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    (s.Description != null && s.Description.Contains(search)));
                ViewBag.Search = search;
            }

            // Apply genre filter
            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(s => s.Genre == genre);
                ViewBag.Genre = genre;
            }

            // Get distinct genres for filter dropdown
            ViewBag.Genres = await _context.Shows
                .Where(s => s.IsActive)
                .Select(s => s.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            // Pagination
            var totalItems = await query.CountAsync();
            var shows = await query
                .Include(s => s.Episodes)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Guest/Home/Shows.cshtml", shows);
        }

        // GET: Guest/Home/ShowDetails/5
        public async Task<IActionResult> ShowDetails(int id)
        {
            var show = await _context.Shows
                .Include(s => s.GenreNavigation)
                .Include(s => s.Episodes.OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber))
                .Include(s => s.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (show == null)
            {
                return NotFound();
            }

            // Increment view count
            show.ViewsCount++;
            await _context.SaveChangesAsync();

            // Get related shows (same genre)
            var relatedShows = await _context.Shows
                .Where(s => s.Genre == show.Genre && s.Id != id && s.IsActive)
                .OrderByDescending(s => s.Rating)
                .Take(6)
                .ToListAsync();

            var viewModel = new GuestShowDetailsViewModel
            {
                Show = show,
                EpisodesBySeason = show.Episodes
                    .GroupBy(e => e.SeasonNumber)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                RelatedShows = relatedShows,
                Reviews = show.Reviews
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList()
            };

            return View("~/Views/Guest/Home/ShowDetails.cshtml", viewModel);
        }

        // GET: Guest/Home/Anime
        public async Task<IActionResult> Anime(string search, string genre, int page = 1)
        {
            int pageSize = 12;
            // Filter shows with Anime genre
            var query = _context.Shows.Where(s => s.IsActive && s.Genre == "Anime");

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    (s.Description != null && s.Description.Contains(search)));
                ViewBag.Search = search;
            }

            // Apply genre filter (for sub-genres of anime)
            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(s => s.Genre == genre);
                ViewBag.Genre = genre;
            }

            // Get distinct genres for filter dropdown (within anime)
            ViewBag.Genres = await _context.Shows
                .Where(s => s.IsActive && s.Genre == "Anime")
                .Select(s => s.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            // Pagination
            var totalItems = await query.CountAsync();
            var animes = await query
                .Include(s => s.Episodes)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Guest/Home/Anime.cshtml", animes);
        }

        // GET: Guest/Home/Search
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            // Search in movies
            var movies = await _context.Movies
                .Where(m => m.IsActive &&
                    (m.Title.Contains(query) ||
                     (m.Description != null && m.Description.Contains(query)) ||
                     (m.Cast != null && m.Cast.Contains(query)) ||
                     (m.Director != null && m.Director.Contains(query))))
                .Take(10)
                .ToListAsync();

            // Search in shows
            var shows = await _context.Shows
                .Where(s => s.IsActive &&
                    (s.Title.Contains(query) ||
                     (s.Description != null && s.Description.Contains(query))))
                .Take(10)
                .ToListAsync();

            // Search in anime (shows with Anime genre)
            var animes = await _context.Shows
                .Where(s => s.IsActive && s.Genre == "Anime" &&
                    (s.Title.Contains(query) ||
                     (s.Description != null && s.Description.Contains(query))))
                .Take(10)
                .ToListAsync();

            var viewModel = new GuestSearchViewModel
            {
                Query = query,
                Movies = movies,
                Shows = shows,
                Animes = animes,
                TotalResults = movies.Count + shows.Count + animes.Count
            };

            return View("~/Views/Guest/Home/Search.cshtml", viewModel);
        }

        // GET: Guest/Home/About
        public IActionResult About()
        {
            return View("~/Views/Guest/Home/About.cshtml");
        }

        // GET: Guest/Home/Contact
        public IActionResult Contact()
        {
            return View("~/Views/Guest/Home/Contact.cshtml");
        }

        // POST: Guest/Home/Contact
        [HttpPost]
        public async Task<IActionResult> Contact(string name, string email, string message)
        {
            // Here you would typically send an email or save to database
            TempData["Success"] = "Thank you for contacting us! We'll get back to you soon.";
            return RedirectToAction("Contact");
        }

        // GET: Guest/Home/Privacy
        public IActionResult Privacy()
        {
            return View("~/Views/Guest/Home/Privacy.cshtml");
        }

        // GET: Guest/Home/Terms
        public IActionResult Terms()
        {
            return View("~/Views/Guest/Home/Terms.cshtml");
        }
    }
}