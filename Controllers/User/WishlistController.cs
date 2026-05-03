using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to get current user ID
        private int? GetCurrentUserId()  
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        // GET: User/Wishlist
        public async Task<IActionResult> Index(string filter = "all", int page = 1)
        {
            // Use the helper method to get user ID
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                // Log for debugging
                Console.WriteLine("Wishlist Index: No UserId found, redirecting to login");
                return RedirectToAction("Login", "Auth", new { area = "Guest" });
            }

            Console.WriteLine($"Wishlist Index: UserId {userId} found");

            int pageSize = 20;

            var query = _context.Wishlists
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .Where(w => w.UserId == userId && w.IsActive);

            // Apply filters
            switch (filter)
            {
                case "movies":
                    query = query.Where(w => w.ItemType == "Movie");
                    break;
                case "shows":
                    query = query.Where(w => w.ItemType == "Show" && w.Show.Genre != "Anime");
                    break;
                case "anime":
                    query = query.Where(w => w.ItemType == "Show" && w.Show.Genre == "Anime");
                    break;
                case "watched":
                    query = query.Where(w => w.IsWatched);
                    break;
                case "unwatched":
                    query = query.Where(w => !w.IsWatched);
                    break;
                case "high":
                    query = query.Where(w => w.Priority == 1 && !w.IsWatched);
                    break;
                default:
                    // all - no filter
                    break;
            }

            // Sorting
            query = query.OrderByDescending(w => w.Priority)
                         .ThenByDescending(w => w.AddedDate);

            // Pagination
            var totalItems = await query.CountAsync();
            var wishlist = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get statistics
            var stats = new
            {
                Total = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsActive),
                Movies = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.ItemType == "Movie" && w.IsActive),
                Shows = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.ItemType == "Show" && w.IsActive && w.Show.Genre != "Anime"),
                Anime = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.ItemType == "Show" && w.IsActive && w.Show.Genre == "Anime"),
                Watched = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsWatched),
                Unwatched = await _context.Wishlists.CountAsync(w => w.UserId == userId && !w.IsWatched),
                HighPriority = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.Priority == 1 && !w.IsWatched)
            };

            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Stats = stats;

            return View("~/Views/User/Wishlist/Index.cshtml", wishlist);
        }

        // POST: User/Wishlist/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int? movieId, int? showId, int priority = 1, string notes = "")
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login first", redirect = "/Auth/Login" });
            }

            if (!movieId.HasValue && !showId.HasValue)
            {
                return Json(new { success = false, message = "Please select a movie or show" });
            }

            var existing = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId &&
                    ((movieId.HasValue && w.MovieId == movieId) ||
                     (showId.HasValue && w.ShowId == showId)));

            if (existing != null)
            {
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    existing.AddedDate = DateTime.Now;
                    existing.Notes = notes;
                    existing.Priority = priority;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Added to wishlist!" });
                }
                return Json(new { success = false, message = "Already in wishlist" });
            }

            var itemType = movieId.HasValue ? "Movie" : "Show";

            // If it's a show, check if it's anime
            if (showId.HasValue)
            {
                var show = await _context.Shows.FindAsync(showId);
                if (show != null && show.Genre == "Anime")
                {
                    itemType = "Anime";
                }
            }

            var wishlist = new Wishlist
            {
                UserId = userId.Value,
                MovieId = movieId,
                ShowId = showId,
                ItemType = itemType,
                AddedDate = DateTime.Now,
                Notes = notes,
                Priority = priority,
                IsActive = true,
                NotificationEnabled = true
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Added to wishlist!",
                count = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsActive)
            });
        }

        // POST: User/Wishlist/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login first" });
            }

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (wishlist == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            wishlist.IsActive = false;
            wishlist.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            var newCount = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsActive);

            return Json(new
            {
                success = true,
                message = "Removed from wishlist",
                count = newCount
            });
        }

        // POST: User/Wishlist/MarkAsWatched
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsWatched(int id, decimal? rating = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login first" });
            }

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (wishlist == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            wishlist.IsWatched = true;
            wishlist.WatchedDate = DateTime.Now;
            wishlist.UserRating = rating;

            if (wishlist.ItemType == "Movie" && wishlist.MovieId.HasValue)
            {
                var activity = new UserActivity
                {
                    UserId = userId.Value,
                    MovieId = wishlist.MovieId,
                    ActivityType = "Watch",
                    ActivityDate = DateTime.Now,
                    IsCompleted = true,
                    ProgressPercentage = 100
                };
                _context.UserActivities.Add(activity);

                var movie = await _context.Movies.FindAsync(wishlist.MovieId);
                if (movie != null)
                {
                    movie.ViewsCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Marked as watched!" });
        }

        // GET: User/Wishlist/GetWishlistCount
        public async Task<IActionResult> GetWishlistCount()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(0);
            }

            var count = await _context.Wishlists
                .CountAsync(w => w.UserId == userId && w.IsActive);
            return Json(count);
        }

        // Test method to check session
        public IActionResult TestSession()
        {
            var userId = GetCurrentUserId();
            var userName = User.Identity?.Name ?? "Unknown";

            if (userId == null)
            {
                return Content("❌ No user logged in");
            }

            return Content($"✅ Logged in! UserId: {userId}, UserName: {userName}");
        }
    }
}