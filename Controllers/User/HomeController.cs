using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;

namespace MovieMania.Controllers.User
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "" });
            }

            var user = await _context.Users.FindAsync(userId);

            // Fetch all movies from database
            var allMovies = await _context.Movies
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            // Fetch all shows from database
            var allShows = await _context.Shows
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            // Fetch anime (shows with Anime genre)
            var animeShows = allShows.Where(s => s.Genre == "Anime").ToList();

            // Fetch user's wishlist
            var userWishlist = await _context.Wishlists
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .Where(w => w.UserId == userId && w.IsActive)
                .OrderByDescending(w => w.AddedDate)
                .Take(8)
                .ToListAsync();

            // Fetch continue watching items
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
                UserName = user?.Name ?? "User",
                AllMovies = allMovies,
                AllShows = allShows,
                AnimeShows = animeShows,
                UserWishlist = userWishlist,
                ContinueWatching = continueWatching,
                TotalWishlistCount = totalWishlistCount,
                RecentlyWatchedCount = recentlyWatchedCount,
                TotalWatchTimeMinutes = totalWatchTime / 60,
                ActiveSubscription = activeSubscription,
                RecommendedMovies = new List<Movie>(),
                TrendingMovies = new List<Movie>(),
                TrendingShows = new List<Show>()
            };

            return View("~/Views/User/Home/Index.cshtml", viewModel);
        }

        public async Task<IActionResult> MovieDetails(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
            {
                return NotFound();
            }

            // Increment view count
            movie.ViewsCount++;
            await _context.SaveChangesAsync();

            // ✅ Show details page first (don't redirect immediately)
            return View("~/Views/User/Home/MovieDetails.cshtml", movie);
        }

        // ✅ New action for playing video when "Watch Now" is clicked
        public async Task<IActionResult> PlayMovie(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null || string.IsNullOrEmpty(movie.VideoUrl))
            {
                return NotFound();
            }

            // You could track play count here if needed
            // movie.PlayCount++; (if you add such a field)

            // Redirect to the external video URL (YouTube, Vimeo, etc.)
            return Redirect(movie.VideoUrl);
        }

        public async Task<IActionResult> ShowDetails(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (show == null)
            {
                return NotFound();
            }

            // Increment view count
            show.ViewsCount++;
            await _context.SaveChangesAsync();

            return View("~/Views/User/Home/ShowDetails.cshtml", show);
        }

        public async Task<IActionResult> PlayEpisode(int id)
        {
            var episode = await _context.Episodes
                .Include(e => e.Show)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            if (episode == null || string.IsNullOrEmpty(episode.VideoUrl))
            {
                return NotFound();
            }

            // Redirect to the external video URL
            return Redirect(episode.VideoUrl);
        }

        public async Task<IActionResult> Movies()
        {
            var movies = await _context.Movies
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return View("~/Views/User/Home/Movies.cshtml", movies);
        }

        public async Task<IActionResult> Shows()
        {
            var shows = await _context.Shows
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View("~/Views/User/Home/Shows.cshtml", shows);
        }

        public async Task<IActionResult> Anime()
        {
            var anime = await _context.Shows
                .Where(s => s.IsActive && s.Genre == "Anime")
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View("~/Views/User/Home/Anime.cshtml", anime);
        }
    }
}