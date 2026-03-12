using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MovieMania.Controllers.Guest
{
    [Route("")]
    [Route("Home")]
    public class GuestHomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestHomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var featuredMovies = await _context.Movies
                    .Where(m => m.IsActive && m.IsFeatured)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                var latestMovies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(12)
                    .ToListAsync();

                var popularMovies = await _context.Movies
                    .Where(m => m.IsActive && m.Rating >= 7)
                    .OrderByDescending(m => m.ViewsCount)
                    .Take(8)
                    .ToListAsync();

                var trendingShows = await _context.Shows
                    .Include(s => s.Episodes)
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.ViewsCount)
                    .Take(6)
                    .ToListAsync();

                var genres = await _context.Genres
                    .Where(g => g.IsActive)
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
                Console.WriteLine($"Error in GuestHomeController.Index: {ex.Message}");

                var emptyViewModel = new GuestHomeViewModel();
                return View("~/Views/Guest/Home/Index.cshtml", emptyViewModel);
            }
        }

        [Route("Movies")]
        public async Task<IActionResult> Movies(string search, string genre, int page = 1)
        {
            int pageSize = 12;
            var query = _context.Movies.Where(m => m.IsActive);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.Title.Contains(search) ||
                    m.Description.Contains(search) ||
                    m.Cast.Contains(search) ||
                    m.Director.Contains(search));
                ViewBag.Search = search;
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(m => m.Genre == genre);
                ViewBag.Genre = genre;
            }

            var totalItems = await query.CountAsync();
            var movies = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Genres = await _context.Movies
                .Where(m => m.IsActive)
                .Select(m => m.Genre)
                .Distinct()
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Guest/Home/Movies.cshtml", movies);
        }

        [Route("MovieDetails/{id}")]
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

            movie.ViewsCount++;
            await _context.SaveChangesAsync();

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

        [Route("Shows")]
        public async Task<IActionResult> Shows(string search, string genre, int page = 1)
        {
            int pageSize = 12;
            var query = _context.Shows.Where(s => s.IsActive);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    s.Description.Contains(search));
                ViewBag.Search = search;
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(s => s.Genre == genre);
                ViewBag.Genre = genre;
            }

            var totalItems = await query.CountAsync();

            var shows = await query
                .Include(s => s.Episodes)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Genres = await _context.Shows
                .Where(s => s.IsActive)
                .Select(s => s.Genre)
                .Distinct()
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

            return View("~/Views/Guest/Home/Shows.cshtml", shows);
        }

        [Route("ShowDetails/{id}")]
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

            show.ViewsCount++;
            await _context.SaveChangesAsync();

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

        [Route("Search")]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            var movies = await _context.Movies
                .Where(m => m.IsActive &&
                    (m.Title.Contains(query) ||
                     m.Description.Contains(query) ||
                     m.Cast.Contains(query) ||
                     m.Director.Contains(query)))
                .Take(10)
                .ToListAsync();

            var shows = await _context.Shows
                .Where(s => s.IsActive &&
                    (s.Title.Contains(query) ||
                     s.Description.Contains(query)))
                .Take(10)
                .ToListAsync();

            var viewModel = new GuestSearchViewModel
            {
                Query = query,
                Movies = movies,
                Shows = shows,
                TotalResults = movies.Count + shows.Count
            };

            return View("~/Views/Guest/Home/Search.cshtml", viewModel);
        }

        [Route("About")]
        public IActionResult About()
        {
            return View("~/Views/Guest/Home/About.cshtml");
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            return View("~/Views/Guest/Home/Contact.cshtml");
        }

        [HttpPost]
        [Route("Contact")]
        public async Task<IActionResult> Contact(string name, string email, string message)
        {
            TempData["Success"] = "Thank you for contacting us! We'll get back to you soon.";
            return RedirectToAction("Contact");
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View("~/Views/Guest/Home/Privacy.cshtml");
        }

        [Route("Terms")]
        public IActionResult Terms()
        {
            return View("~/Views/Guest/Home/Terms.cshtml");
        }
    }
}