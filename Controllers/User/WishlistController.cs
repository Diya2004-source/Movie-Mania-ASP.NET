<<<<<<< HEAD
﻿//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MovieMania.Models;
//using System.Security.Claims;

//namespace MovieMania.Controllers.User
//{
//    [Authorize(Roles = "user")]
//    public class WishlistController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public WishlistController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        private int? GetCurrentUserId()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) ? userId : null;
//        }

//        // GET: User/Wishlist
//        public async Task<IActionResult> Index(string filter = "all", int page = 1)
//        {
//            var userId = GetCurrentUserId();
//            if (userId == null)
//            {
//                return RedirectToAction("Login", "Auth", new { area = "Guest" });
//            }

//            int pageSize = 20;
//            var query = _context.Wishlists
//                .Include(w => w.Movie)
//                .Include(w => w.Show)
//                .Where(w => w.UserId == userId && w.IsActive);

//            // Apply filters
//            query = filter.ToLower() switch
//            {
//                "movies" => query.Where(w => w.ItemType == "Movie"),
//                "shows" => query.Where(w => w.ItemType == "Show" && w.Show.Genre != "Anime"),
//                "anime" => query.Where(w => w.ItemType == "Show" && w.Show.Genre == "Anime"),
//                "watched" => query.Where(w => w.IsWatched),
//                "unwatched" => query.Where(w => !w.IsWatched),
//                "high" => query.Where(w => w.Priority == 1 && !w.IsWatched),
//                _ => query // "all"
//            };

//            var totalItems = await query.CountAsync();
//            var wishlist = await query
//                .OrderByDescending(w => w.Priority)
//                .ThenByDescending(w => w.AddedDate)
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();

//            ViewBag.CurrentFilter = filter;
//            ViewBag.CurrentPage = page;
//            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

//            // Fixed: This now uses a single, safe database call
//            ViewBag.Stats = await GetWishlistStats(userId.Value);

//            return View("~/Views/User/Wishlist/Index.cshtml", wishlist);
//        }

//        private async Task<object> GetWishlistStats(int userId)
//        {
//            // This grouping logic translates to a single "SELECT COUNT(CASE...)" SQL statement
//            var stats = await _context.Wishlists
//                .Where(w => w.UserId == userId)
//                .GroupBy(w => 1)
//                .Select(g => new
//                {
//                    Total = g.Count(w => w.IsActive),
//                    Movies = g.Count(w => w.IsActive && w.ItemType == "Movie"),
//                    Shows = g.Count(w => w.IsActive && w.ItemType == "Show" && w.Show.Genre != "Anime"),
//                    Anime = g.Count(w => w.IsActive && w.ItemType == "Show" && w.Show.Genre == "Anime"),
//                    Watched = g.Count(w => w.IsWatched),
//                    Unwatched = g.Count(w => !w.IsWatched),
//                    HighPriority = g.Count(w => w.IsActive && w.Priority == 1 && !w.IsWatched)
//                })
//                .FirstOrDefaultAsync();

//            // Return default zeros if no items exist yet
//            return stats ?? new { Total = 0, Movies = 0, Shows = 0, Anime = 0, Watched = 0, Unwatched = 0, HighPriority = 0 };
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Add(int? movieId, int? showId, int priority = 1, string notes = "")
//        {
//            var userId = GetCurrentUserId();
//            if (userId == null) return Json(new { success = false, message = "Session expired", redirect = "/Auth/Login" });

//            if (!movieId.HasValue && !showId.HasValue)
//                return Json(new { success = false, message = "Invalid selection" });

//            var existing = await _context.Wishlists
//                .FirstOrDefaultAsync(w => w.UserId == userId &&
//                    ((movieId.HasValue && w.MovieId == movieId) || (showId.HasValue && w.ShowId == showId)));

//            if (existing != null)
//            {
//                if (!existing.IsActive)
//                {
//                    existing.IsActive = true;
//                    existing.AddedDate = DateTime.Now;
//                    existing.Notes = notes;
//                    existing.Priority = priority;
//                    await _context.SaveChangesAsync();
//                    return Json(new { success = true, message = "Item restored to wishlist!" });
//                }
//                return Json(new { success = false, message = "Already in wishlist" });
//            }

//            string itemType = "Movie";
//            if (showId.HasValue)
//            {
//                var show = await _context.Shows.FindAsync(showId);
//                itemType = (show?.Genre == "Anime") ? "Anime" : "Show";
//            }

//            var newItem = new Wishlist
//            {
//                UserId = userId.Value,
//                MovieId = movieId,
//                ShowId = showId,
//                ItemType = itemType,
//                AddedDate = DateTime.Now,
//                Notes = notes,
//                Priority = priority,
//                IsActive = true,
//                NotificationEnabled = true
//            };

//            _context.Wishlists.Add(newItem);
//            await _context.SaveChangesAsync();

//            return Json(new { success = true, message = "Added!", count = await GetActiveCount(userId.Value) });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Remove(int id)
//        {
//            var userId = GetCurrentUserId();
//            var item = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

//            if (item == null) return Json(new { success = false, message = "Item not found" });

//            item.IsActive = false;
//            item.UpdatedDate = DateTime.Now;
//            await _context.SaveChangesAsync();

//            return Json(new { success = true, message = "Removed", count = await GetActiveCount(userId.Value) });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> MarkAsWatched(int id, decimal? rating = null)
//        {
//            var userId = GetCurrentUserId();
//            var item = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

//            if (item == null) return Json(new { success = false, message = "Item not found" });

//            item.IsWatched = true;
//            item.WatchedDate = DateTime.Now;
//            item.UserRating = rating;

//            if (item.ItemType == "Movie" && item.MovieId.HasValue)
//            {
//                _context.UserActivities.Add(new UserActivity
//                {
//                    UserId = userId.Value,
//                    MovieId = item.MovieId,
//                    ActivityType = "Watch",
//                    ActivityDate = DateTime.Now,
//                    IsCompleted = true,
//                    ProgressPercentage = 100
//                });

//                var movie = await _context.Movies.FindAsync(item.MovieId);
//                if (movie != null) movie.ViewsCount++;
//            }

//            await _context.SaveChangesAsync();
//            return Json(new { success = true, message = "Enjoyed your watch!" });
//        }

//        private async Task<int> GetActiveCount(int userId) =>
//            await _context.Wishlists.CountAsync(w => w.UserId == userId && w.IsActive);

//        public async Task<IActionResult> GetWishlistCount() =>
//            Json(await GetActiveCount(GetCurrentUserId() ?? 0));
//    }
//}

using Microsoft.AspNetCore.Authorization;
=======
﻿using Microsoft.AspNetCore.Authorization;
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
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

<<<<<<< HEAD
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            return (userIdClaim != null &&
                    int.TryParse(userIdClaim.Value, out int userId))
                ? userId
                : null;
        }

        // =========================
        // GET: USER/WISHLIST
        // =========================

        public async Task<IActionResult> Index(string filter = "all", int page = 1)
        {
=======
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

        //Methid to GET: User/Wishlist
        public async Task<IActionResult> Index(string filter = "all", int page = 1)
        {
            // Use the helper method to get user ID
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
            var userId = GetCurrentUserId();

            if (userId == null)
            {
<<<<<<< HEAD
                return RedirectToAction("Login", "Auth");
            }

=======
                // Log for debugging
                Console.WriteLine("Wishlist Index: No UserId found, redirecting to login");
                return RedirectToAction("Login", "Auth", new { area = "Guest" });
            }

            Console.WriteLine($"Wishlist Index: UserId {userId} found");

>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
            int pageSize = 20;

            var query = _context.Wishlists
                .Include(w => w.Movie)
                .Include(w => w.Show)
                .Where(w => w.UserId == userId && w.IsActive);

<<<<<<< HEAD
            // FILTERS
            query = filter.ToLower() switch
            {
                "movies" => query.Where(w => w.ItemType == "Movie"),

                "shows" => query.Where(w =>
                    w.ItemType == "Show" &&
                    w.Show.Genre != "Anime"),

                "anime" => query.Where(w =>
                    w.ItemType == "Show" &&
                    w.Show.Genre == "Anime"),

                "watched" => query.Where(w => w.IsWatched),

                "unwatched" => query.Where(w => !w.IsWatched),

                "high" => query.Where(w =>
                    w.Priority == 1 &&
                    !w.IsWatched),

                _ => query
            };

            var totalItems = await query.CountAsync();

            var wishlist = await query
                .OrderByDescending(w => w.Priority)
                .ThenByDescending(w => w.AddedDate)
=======
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

            //Apply Sorting
            query = query.OrderByDescending(w => w.Priority)
                         .ThenByDescending(w => w.AddedDate);

            // Pagination
            var totalItems = await query.CountAsync();
            var wishlist = await query
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

<<<<<<< HEAD
            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages =
                (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.Stats = await GetWishlistStats(userId.Value);
=======
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
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d

            return View("~/Views/User/Wishlist/Index.cshtml", wishlist);
        }

<<<<<<< HEAD
        // =========================
        // WISHLIST STATS
        // =========================

        private async Task<object> GetWishlistStats(int userId)
        {
            var stats = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .GroupBy(w => 1)
                .Select(g => new
                {
                    Total = g.Count(w => w.IsActive),

                    Movies = g.Count(w =>
                        w.IsActive &&
                        w.ItemType == "Movie"),

                    Shows = g.Count(w =>
                        w.IsActive &&
                        w.ItemType == "Show" &&
                        w.Show.Genre != "Anime"),

                    Anime = g.Count(w =>
                        w.IsActive &&
                        w.ItemType == "Show" &&
                        w.Show.Genre == "Anime"),

                    Watched = g.Count(w => w.IsWatched),

                    Unwatched = g.Count(w => !w.IsWatched),

                    HighPriority = g.Count(w =>
                        w.IsActive &&
                        w.Priority == 1 &&
                        !w.IsWatched)
                })
                .FirstOrDefaultAsync();

            return stats ?? new
            {
                Total = 0,
                Movies = 0,
                Shows = 0,
                Anime = 0,
                Watched = 0,
                Unwatched = 0,
                HighPriority = 0
            };
        }

        // =========================
        // ADD TO WISHLIST
        // =========================

        [HttpPost]
        public async Task<IActionResult> Add(
            int? movieId,
            int? showId,
            int priority = 1,
            string notes = "")
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Session expired",
                    redirect = "/Auth/Login"
                });
=======
        // POST: User/Wishlist/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int? movieId, int? showId, int priority = 1, string notes = "")
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login first", redirect = "/Auth/Login" });
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
            }

            if (!movieId.HasValue && !showId.HasValue)
            {
<<<<<<< HEAD
                return Json(new
                {
                    success = false,
                    message = "Invalid selection"
                });
            }

            var existing = await _context.Wishlists
                .FirstOrDefaultAsync(w =>
                    w.UserId == userId &&
                    (
                        (movieId.HasValue && w.MovieId == movieId) ||
                        (showId.HasValue && w.ShowId == showId)
                    ));
=======
                return Json(new { success = false, message = "Please select a movie or show" });
            }

            var existing = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId &&
                    ((movieId.HasValue && w.MovieId == movieId) ||
                     (showId.HasValue && w.ShowId == showId)));
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d

            if (existing != null)
            {
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    existing.AddedDate = DateTime.Now;
                    existing.Notes = notes;
                    existing.Priority = priority;
<<<<<<< HEAD

                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Item restored to wishlist!"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Already in wishlist"
                });
            }

            string itemType = "Movie";

            if (showId.HasValue)
            {
                var show = await _context.Shows.FindAsync(showId);

                itemType = (show?.Genre == "Anime")
                    ? "Anime"
                    : "Show";
            }

            var newItem = new Wishlist
=======
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
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
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

<<<<<<< HEAD
            _context.Wishlists.Add(newItem);

=======
            _context.Wishlists.Add(wishlist);
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
<<<<<<< HEAD
                message = "Added!",
                count = await GetActiveCount(userId.Value)
            });
        }

        // =========================
        // REMOVE FROM WISHLIST
        // =========================

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            var item = await _context.Wishlists
                .FirstOrDefaultAsync(w =>
                    w.Id == id &&
                    w.UserId == userId);

            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Item not found"
                });
            }

            item.IsActive = false;
            item.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Removed successfully",
                count = await GetActiveCount(userId.Value)
            });
        }

        // =========================
        // MARK AS WATCHED
        // =========================

        [HttpPost]
        public async Task<IActionResult> MarkAsWatched(
            int id,
            decimal? rating = null)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            var item = await _context.Wishlists
                .FirstOrDefaultAsync(w =>
                    w.Id == id &&
                    w.UserId == userId);

            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Item not found"
                });
            }

            item.IsWatched = true;
            item.WatchedDate = DateTime.Now;
            item.UserRating = rating;

            // ADD USER ACTIVITY
            if (item.ItemType == "Movie" && item.MovieId.HasValue)
            {
                _context.UserActivities.Add(new UserActivity
                {
                    UserId = userId.Value,
                    MovieId = item.MovieId,
=======
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
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                    ActivityType = "Watch",
                    ActivityDate = DateTime.Now,
                    IsCompleted = true,
                    ProgressPercentage = 100
<<<<<<< HEAD
                });

                var movie = await _context.Movies.FindAsync(item.MovieId);

=======
                };
                _context.UserActivities.Add(activity);

                var movie = await _context.Movies.FindAsync(wishlist.MovieId);
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
                if (movie != null)
                {
                    movie.ViewsCount++;
                }
            }

            await _context.SaveChangesAsync();

<<<<<<< HEAD
            return Json(new
            {
                success = true,
                message = "Enjoyed your watch!"
            });
        }

        // =========================
        // ACTIVE COUNT
        // =========================

        private async Task<int> GetActiveCount(int userId)
        {
            return await _context.Wishlists
                .CountAsync(w =>
                    w.UserId == userId &&
                    w.IsActive);
        }

        // =========================
        // GET COUNT
        // =========================

        public async Task<IActionResult> GetWishlistCount()
        {
            return Json(
                await GetActiveCount(GetCurrentUserId() ?? 0)
            );
=======
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
                return Content("No user logged in");
            }

            return Content($"✅ Logged in! UserId: {userId}, UserName: {userName}");
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
        }
    }
}