using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;  // Make sure this is here
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MovieMania.Controllers.Guest
{
    public class GuestHomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestHomeController(ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}