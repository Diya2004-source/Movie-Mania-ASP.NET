using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Home/Index - Dashboard
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            // Get user's watch history for personalized recommendations
            var userWatchHistory = await _context.UserActivities
                .Where(ua => ua.UserId == userId && ua.IsCompleted)
                .Select(ua => ua.MovieId)
                .ToListAsync();

            // Get user's favorite genres based on watch history
            var favoriteGenres = await _context.Movies
                .Where(m => userWatchHistory.Contains(m.Id))
                .Select(m => m.Genre)
                .Distinct()
                .ToListAsync();

            // Fetch personalized recommendations
            var recommendedMovies = await _context.Movies
                .Where(m => m.IsActive && m.Rating >= 7 &&
                       (favoriteGenres.Contains(m.Genre) || userWatchHistory.Contains(m.Id)))
                .OrderByDescending(m => m.Rating)
                .Take(10)
                .ToListAsync();

            // Fetch trending movies (by views)
            var trendingMovies = await _context.Movies
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.ViewsCount)
                .Take(8)
                .ToListAsync();

            // Fetch trending shows (by views)
            var trendingShows = await _context.Shows
                .Include(s => s.Episodes)
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.ViewsCount)
                .Take(6)
                .ToListAsync();

            // Get user's wishlist
            var userWishlist = await _context.Wishlists
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .Where(w => w.UserId == userId && w.IsActive)
                .OrderByDescending(w => w.AddedDate)
                .Take(8)
                .ToListAsync();

            // Get continue watching items
            var continueWatching = await _context.UserActivities
                .Include(ua => ua.Movie)
                .Include(ua => ua.Episode)
                    .ThenInclude(e => e.Show)
                .Where(ua => ua.UserId == userId && !ua.IsCompleted && ua.ProgressPercentage > 0)
                .OrderByDescending(ua => ua.ActivityDate)
                .Take(6)
                .ToListAsync();

            // Get user statistics
            var totalWishlistCount = await _context.Wishlists
                .CountAsync(w => w.UserId == userId && w.IsActive);

            var recentlyWatchedCount = await _context.UserActivities
                .CountAsync(ua => ua.UserId == userId && ua.IsCompleted);

            var totalWatchTime = await _context.UserActivities
                .Where(ua => ua.UserId == userId)
                .SumAsync(ua => (int?)ua.WatchDuration) ?? 0;

            // Get active subscription
            var activeSubscription = await _context.UserSubscriptions
                .Include(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync(us => us.UserId == userId && us.Status == "Active");

            var viewModel = new UserDashboardViewModel
            {
                RecommendedMovies = recommendedMovies,
                TrendingMovies = trendingMovies,
                TrendingShows = trendingShows,
                UserWishlist = userWishlist,
                ContinueWatching = continueWatching,
                TotalWishlistCount = totalWishlistCount,
                RecentlyWatchedCount = recentlyWatchedCount,
                TotalWatchTimeMinutes = totalWatchTime / 60,
                ActiveSubscription = activeSubscription,
                UserName = User.FindFirstValue(ClaimTypes.Name) ?? "User"
            };

            return View(viewModel);
        }

        // GET: User/Home/Movies
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

            // Check which movies are in user's wishlist
            var userWishlistIds = await _context.Wishlists
                .Where(w => w.UserId == userId && w.IsActive && w.MovieId != null)
                .Select(w => w.MovieId.Value)
                .ToListAsync();

            ViewBag.UserWishlistIds = userWishlistIds;

            // Pagination
            var totalItems = await query.CountAsync();
            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View(movies);
        }

        // GET: User/Home/MovieDetails/5
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

            // Increment view count
            movie.ViewsCount++;
            await _context.SaveChangesAsync();

            // Check if in wishlist
            var isInWishlist = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.MovieId == id && w.IsActive);

            // Get user rating if exists
            var userRating = await _context.MovieReviews
                .Where(r => r.MovieId == id && r.UserId == userId)
                .Select(r => (decimal?)r.Rating)
                .FirstOrDefaultAsync();

            // Get related movies (same genre)
            var relatedMovies = await _context.Movies
                .Where(m => m.Genre == movie.Genre && m.Id != id && m.IsActive)
                .OrderByDescending(m => m.Rating)
                .Take(6)
                .ToListAsync();

            // Check related movies wishlist status
            var relatedMovieIds = relatedMovies.Select(m => m.Id).ToList();
            var wishlistStatus = await _context.Wishlists
                .Where(w => w.UserId == userId && w.IsActive && relatedMovieIds.Contains(w.MovieId.Value))
                .Select(w => w.MovieId.Value)
                .ToListAsync();

            var viewModel = new UserMovieDetailsViewModel
            {
                Movie = movie,
                IsInWishlist = isInWishlist,
                UserRating = userRating,
                RelatedMovies = relatedMovies,
                RelatedMoviesInWishlist = wishlistStatus,
                Reviews = movie.Reviews
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: User/Home/Shows
        public async Task<IActionResult> Shows(string search, string genre, int page = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            int pageSize = 18;
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

            // Check which shows are in user's wishlist
            var userWishlistIds = await _context.Wishlists
                .Where(w => w.UserId == userId && w.IsActive && w.ShowId != null)
                .Select(w => w.ShowId.Value)
                .ToListAsync();

            ViewBag.UserWishlistIds = userWishlistIds;

            // Pagination
            var totalItems = await query.CountAsync();
            var shows = await query
                .Include(s => s.Episodes)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View(shows);
        }

        // GET: User/Home/ShowDetails/5
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

            // Increment view count
            show.ViewsCount++;
            await _context.SaveChangesAsync();

            // Check if in wishlist
            var isInWishlist = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ShowId == id && w.IsActive);

            // Get watched episodes
            var watchedEpisodes = await _context.UserActivities
                .Where(ua => ua.UserId == userId && ua.Episode.ShowId == id && ua.IsCompleted && ua.EpisodeId.HasValue)
                .Select(ua => ua.EpisodeId.Value)
                .ToListAsync();

            // Get related shows (same genre)
            var relatedShows = await _context.Shows
                .Where(s => s.Genre == show.Genre && s.Id != id && s.IsActive)
                .OrderByDescending(s => s.Rating)
                .Take(6)
                .ToListAsync();

            // Check related shows wishlist status
            var relatedShowIds = relatedShows.Select(s => s.Id).ToList();
            var wishlistStatus = await _context.Wishlists
                .Where(w => w.UserId == userId && w.IsActive && relatedShowIds.Contains(w.ShowId.Value))
                .Select(w => w.ShowId.Value)
                .ToListAsync();

            var viewModel = new UserShowDetailsViewModel
            {
                Show = show,
                IsInWishlist = isInWishlist,
                EpisodesBySeason = show.Episodes
                    .GroupBy(e => e.SeasonNumber)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                WatchedEpisodes = watchedEpisodes,
                WatchProgress = show.Episodes.Any()
                    ? (watchedEpisodes.Count * 100 / show.Episodes.Count)
                    : 0,
                RelatedShows = relatedShows,
                RelatedShowsInWishlist = wishlistStatus,
                Reviews = show.Reviews
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: User/Home/Categories
        public async Task<IActionResult> Categories()
        {
            var genres = await _context.Genres
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .ToListAsync();

            var moviesByGenre = new Dictionary<string, List<Movie>>();
            var showsByGenre = new Dictionary<string, List<Show>>();

            foreach (var genre in genres)
            {
                moviesByGenre[genre.Name] = await _context.Movies
                    .Where(m => m.Genre == genre.Name && m.IsActive)
                    .OrderByDescending(m => m.Rating)
                    .Take(8)
                    .ToListAsync();

                showsByGenre[genre.Name] = await _context.Shows
                    .Where(s => s.Genre == genre.Name && s.IsActive)
                    .OrderByDescending(s => s.Rating)
                    .Take(8)
                    .ToListAsync();
            }

            var viewModel = new UserCategoriesViewModel
            {
                Genres = genres,
                MoviesByGenre = moviesByGenre,
                ShowsByGenre = showsByGenre,
                PopularTags = new List<string>
                {
                    "#Action", "#Comedy", "#Drama", "#SciFi", "#Thriller",
                    "#Romance", "#Horror", "#Documentary", "#Animation",
                    "#Marvel", "#DC", "#Anime", "#Netflix", "#HBO"
                }
            };

            return View(viewModel);
        }

        // POST: User/Home/UpdateWatchProgress
        [HttpPost]
        public async Task<IActionResult> UpdateWatchProgress(int? movieId, int? episodeId, int progress, int position)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            UserActivity activity = null;

            if (movieId.HasValue)
            {
                activity = await _context.UserActivities
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.MovieId == movieId);
            }
            else if (episodeId.HasValue)
            {
                activity = await _context.UserActivities
                    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.EpisodeId == episodeId);
            }

            if (activity == null)
            {
                activity = new UserActivity
                {
                    UserId = userId,
                    MovieId = movieId,
                    EpisodeId = episodeId,
                    ActivityType = movieId.HasValue ? "Movie" : "Episode",
                    ActivityDate = DateTime.Now,
                    ProgressPercentage = progress,
                    LastPosition = TimeSpan.FromSeconds(position),
                    IsCompleted = progress >= 95
                };
                _context.UserActivities.Add(activity);
            }
            else
            {
                activity.ProgressPercentage = progress;
                activity.LastPosition = TimeSpan.FromSeconds(position);
                activity.ActivityDate = DateTime.Now;
                activity.IsCompleted = progress >= 95;
            }

            if (activity.IsCompleted)
            {
                if (movieId.HasValue)
                {
                    var movie = await _context.Movies.FindAsync(movieId);
                    if (movie != null) movie.ViewsCount++;
                }
                else if (episodeId.HasValue)
                {
                    var episode = await _context.Episodes.FindAsync(episodeId);
                    if (episode != null) episode.ViewsCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                isCompleted = activity.IsCompleted,
                message = activity.IsCompleted ? "Completed! " : "Progress saved"
            });
        }
    }
}