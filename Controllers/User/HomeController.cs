using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    [Area("User")]  // ✅ ADD THIS LINE - This is the fix!
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            var viewModel = new UserDashboardViewModel
            {
                FeaturedMovies = await _context.Movies
                    .Where(m => m.IsActive && m.IsFeatured)
                    .Take(10)
                    .ToListAsync(),

                RecentMovies = await _context.Movies
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(12)
                    .ToListAsync(),

                RecommendedMovies = await _context.Movies
                    .Where(m => m.IsActive && m.Rating >= 7)
                    .OrderByDescending(m => m.Rating)
                    .Take(8)
                    .ToListAsync(),

                TrendingShows = await _context.Shows
                    .Include(s => s.Episodes)
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.ViewsCount)
                    .Take(6)
                    .ToListAsync(),

                UserWishlist = await _context.Wishlists
                    .Include(w => w.Movie)
                    .Include(w => w.Show)
                    .Where(w => w.UserId == userId && w.IsActive)
                    .OrderByDescending(w => w.AddedDate)
                    .Take(8)
                    .ToListAsync(),

                ContinueWatching = await _context.UserActivities
                    .Include(ua => ua.Movie)
                    .Include(ua => ua.Episode)
                        .ThenInclude(e => e.Show)
                    .Where(ua => ua.UserId == userId && !ua.IsCompleted && ua.ProgressPercentage > 0)
                    .OrderByDescending(ua => ua.ActivityDate)
                    .Take(6)
                    .ToListAsync(),

                TotalWishlistCount = await _context.Wishlists
                    .CountAsync(w => w.UserId == userId && w.IsActive),

                RecentlyWatchedCount = await _context.UserActivities
                    .CountAsync(ua => ua.UserId == userId && ua.IsCompleted),

                ActiveSubscription = await _context.UserSubscriptions
                    .Include(us => us.SubscriptionPlan)
                    .FirstOrDefaultAsync(us => us.UserId == userId && us.Status == "Active")
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Movies(string search, string genre, string sortBy = "latest", int page = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            int pageSize = 24;
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

            query = sortBy switch
            {
                "rating" => query.OrderByDescending(m => m.Rating),
                "year" => query.OrderByDescending(m => m.ReleaseYear),
                "title" => query.OrderBy(m => m.Title),
                _ => query.OrderByDescending(m => m.CreatedAt)
            };
            ViewBag.SortBy = sortBy;

            ViewBag.Genres = await _context.Movies
                .Where(m => m.IsActive)
                .Select(m => m.Genre)
                .Distinct()
                .ToListAsync();

            var totalItems = await query.CountAsync();
            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(movies);
        }

        public async Task<IActionResult> MovieDetails(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

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

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                IsInWishlist = await _context.Wishlists
                    .AnyAsync(w => w.UserId == userId && w.MovieId == id && w.IsActive),
                UserRating = await _context.MovieReviews
                    .Where(r => r.MovieId == id && r.UserId == userId)
                    .Select(r => (decimal?)r.Rating)
                    .FirstOrDefaultAsync(),
                RelatedMovies = await _context.Movies
                    .Where(m => m.Genre == movie.Genre && m.Id != id && m.IsActive)
                    .OrderByDescending(m => m.Rating)
                    .Take(6)
                    .ToListAsync(),
                Reviews = movie.Reviews
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ShowDetails(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

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

            // FIXED: Added HasValue check and .Value
            var watchedEpisodes = await _context.UserActivities
                .Where(ua => ua.UserId == userId && ua.Episode.ShowId == id && ua.IsCompleted && ua.EpisodeId.HasValue)
                .Select(ua => ua.EpisodeId.Value)
                .ToListAsync();

            var viewModel = new ShowDetailsViewModel
            {
                Show = show,
                IsInWishlist = await _context.Wishlists
                    .AnyAsync(w => w.UserId == userId && w.ShowId == id && w.IsActive),
                EpisodesBySeason = show.Episodes
                    .GroupBy(e => e.SeasonNumber)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                WatchedEpisodes = watchedEpisodes,
                WatchProgress = show.Episodes.Any()
                    ? (watchedEpisodes.Count * 100 / show.Episodes.Count)
                    : 0,
                RelatedShows = await _context.Shows
                    .Where(s => s.Genre == show.Genre && s.Id != id && s.IsActive)
                    .Take(6)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}