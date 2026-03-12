using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
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

        // GET: User/Wishlist
        public async Task<IActionResult> Index(string filter = "all", int page = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);

            int pageSize = 20;

            var query = _context.Wishlists
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .Where(w => w.UserId == userId && w.IsActive);

            query = filter switch
            {
                "movies" => query.Where(w => w.ItemType == "Movie"),
                "shows" => query.Where(w => w.ItemType == "Show"),
                "watched" => query.Where(w => w.IsWatched),
                "unwatched" => query.Where(w => !w.IsWatched),
                "high" => query.Where(w => w.Priority == 1 && !w.IsWatched),
                _ => query
            };

            query = query.OrderByDescending(w => w.Priority)
                         .ThenByDescending(w => w.AddedDate);

            var totalItems = await query.CountAsync();
            var wishlist = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.Stats = new
            {
                Total = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsActive),
                Movies = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.ItemType == "Movie" && w.IsActive),
                Shows = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.ItemType == "Show" && w.IsActive),
                Watched = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsWatched),
                HighPriority = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.Priority == 1 && !w.IsWatched)
            };

            return View(wishlist);
        }

        // POST: User/Wishlist/Add
        [HttpPost]
        public async Task<IActionResult> Add(int? movieId, int? showId, int priority = 1, string notes = "")
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

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

            var wishlist = new Wishlist
            {
                UserId = userId,
                MovieId = movieId,
                ShowId = showId,
                ItemType = movieId.HasValue ? "Movie" : "Show",
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

        // POST: User/Wishlist/Update
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] WishlistUpdateViewModel model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.Id == model.Id && w.UserId == userId);

            if (wishlist == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            if (model.Priority.HasValue)
                wishlist.Priority = model.Priority.Value;

            if (model.Notes != null)
                wishlist.Notes = model.Notes;

            if (model.NotificationEnabled.HasValue)
                wishlist.NotificationEnabled = model.NotificationEnabled.Value;

            wishlist.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Updated successfully" });
        }

        // POST: User/Wishlist/MarkAsWatched
        [HttpPost]
        public async Task<IActionResult> MarkAsWatched(int id, decimal? rating = null)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

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
                    UserId = userId,
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

            return Json(new { success = true, message = "Marked as watched! 🎉" });
        }

        // POST: User/Wishlist/UpdateProgress
        [HttpPost]
        public async Task<IActionResult> UpdateProgress(int id, int watchedSeasons, int watchedEpisodes)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var wishlist = await _context.Wishlists
                .Include(w => w.Show)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId && w.ItemType == "Show");

            if (wishlist == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            wishlist.WatchedSeasons = watchedSeasons;
            wishlist.WatchedEpisodes = watchedEpisodes;

            if (wishlist.Show != null &&
                watchedSeasons >= wishlist.Show.TotalSeasons &&
                watchedEpisodes >= wishlist.Show.TotalEpisodes)
            {
                wishlist.IsWatched = true;
                wishlist.WatchedDate = DateTime.Now;
            }

            wishlist.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Progress updated",
                isCompleted = wishlist.IsWatched
            });
        }

        // POST: User/Wishlist/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (wishlist == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }

            wishlist.IsActive = false;
            wishlist.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Removed from wishlist",
                count = await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsActive)
            });
        }

        // GET: User/Wishlist/GetWishlistCount
        public async Task<IActionResult> GetWishlistCount()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(0);
            }
            var userId = int.Parse(userIdClaim);

            var count = await _context.Wishlists
                .CountAsync(w => w.UserId == userId && w.IsActive);
            return Json(count);
        }
    }
}