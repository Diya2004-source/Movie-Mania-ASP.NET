using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Security.Claims;

namespace MovieMania.Controllers.User
{
    [Authorize(Roles = "user")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: User/Movies/Rate
        [HttpPost]
        public async Task<IActionResult> RateMovie([FromBody] MovieRatingViewModel model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var review = await _context.MovieReviews
                .FirstOrDefaultAsync(r => r.MovieId == model.MovieId && r.UserId == userId);

            if (review == null)
            {
                review = new MovieReview
                {
                    MovieId = model.MovieId,
                    UserId = userId,
                    Rating = model.Rating,
                    ReviewText = model.Review,
                    ReviewDate = DateTime.Now,
                    IsApproved = false
                };
                _context.MovieReviews.Add(review);
            }
            else
            {
                review.Rating = model.Rating;
                review.ReviewText = model.Review;
                review.ReviewDate = DateTime.Now;
                review.IsApproved = false;
            }

            await _context.SaveChangesAsync();

            var movie = await _context.Movies.FindAsync(model.MovieId);
            if (movie != null)
            {
                movie.Rating = await _context.MovieReviews
                    .Where(r => r.MovieId == model.MovieId && r.IsApproved)
                    .AverageAsync(r => (decimal?)r.Rating) ?? 0;
                await _context.SaveChangesAsync();
            }

            return Json(new
            {
                success = true,
                message = "Thank you for rating! Your review will be visible after approval."
            });
        }

        // POST: User/Movies/MarkAsWatched
        [HttpPost]
        public async Task<IActionResult> MarkAsWatched(int movieId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            var userId = int.Parse(userIdClaim);

            var activity = new UserActivity
            {
                UserId = userId,
                MovieId = movieId,
                ActivityType = "Watch",
                ActivityDate = DateTime.Now,
                IsCompleted = true,
                ProgressPercentage = 100
            };

            _context.UserActivities.Add(activity);

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie != null)
            {
                movie.ViewsCount++;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Marked as watched!" });
        }

        // GET: User/Movies/GetComments/5
        public async Task<IActionResult> GetComments(int movieId)
        {
            var comments = await _context.MovieReviews
                .Include(r => r.User)
                .Where(r => r.MovieId == movieId && r.IsApproved)
                .OrderByDescending(r => r.ReviewDate)
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.ReviewText,
                    r.ReviewDate,
                    UserName = r.User != null ? r.User.Name : "Anonymous",
                    r.HelpfulCount
                })
                .ToListAsync();

            return Json(comments);
        }

        // POST: User/Movies/MarkHelpful
        [HttpPost]
        public async Task<IActionResult> MarkHelpful(int reviewId)
        {
            var review = await _context.MovieReviews.FindAsync(reviewId);
            if (review != null)
            {
                review.HelpfulCount++;
                await _context.SaveChangesAsync();
                return Json(new { success = true, count = review.HelpfulCount });
            }
            return Json(new { success = false });
        }

        // GET: User/Movies/GetSimilar/5
        public async Task<IActionResult> GetSimilar(int movieId)
        {
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return Json(new { success = false });
            }

            var similar = await _context.Movies
                .Where(m => m.Genre == movie.Genre && m.Id != movieId && m.IsActive)
                .OrderByDescending(m => m.Rating)
                .Take(6)
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.ThumbnailUrl,
                    m.ReleaseYear,
                    m.Rating
                })
                .ToListAsync();

            return Json(similar);
        }
    }
}