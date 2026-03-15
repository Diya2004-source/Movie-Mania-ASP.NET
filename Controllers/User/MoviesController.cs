using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

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

        public async Task<IActionResult> Index(string? genre, string? sortBy, int page = 1)
        {
            var query = _context.Movies.Where(m => m.IsActive);

            // Apply genre filter
            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(m => m.Genre != null && m.Genre == genre);
            }

            int totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy switch
            {
                "rating" => query.OrderByDescending(m => m.Rating),
                "latest" => query.OrderByDescending(m => m.CreatedAt),
                "views" => query.OrderByDescending(m => m.ViewsCount),
                _ => query.OrderByDescending(m => m.CreatedAt)
            };

            var movies = await query
                .Skip((page - 1) * 12)
                .Take(12)
                .Select(m => new MovieViewModel
                {
                    Id = m.Id,
                    Title = m.Title ?? "Untitled",
                    ThumbnailUrl = m.ThumbnailUrl ?? "/images/default-movie.jpg",
                    Genre = m.Genre ?? "Unknown",
                    ReleaseYear = m.ReleaseYear,
                    Rating = m.Rating.HasValue ? (double?)m.Rating.Value : null,
                    ViewsCount = m.ViewsCount
                })
                .ToListAsync();

            var genres = await _context.Movies
                .Where(m => m.IsActive && m.Genre != null)
                .Select(m => m.Genre!)
                .Distinct()
                .ToListAsync();

            ViewBag.Movies = movies;
            ViewBag.Genres = genres;
            ViewBag.CurrentGenre = genre ?? "All";
            ViewBag.CurrentSort = sortBy ?? "latest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / 12);

            return View("~/Views/User/Movies/Index.cshtml");
        }

        public async Task<IActionResult> Watch(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if user has active subscription
            var hasSubscription = await _context.UserSubscriptions
                .AnyAsync(s => s.UserId == userId && s.EndDate >= DateTime.Now && s.IsActive);

            if (!hasSubscription)
            {
                TempData["Error"] = "Please subscribe to watch movies";
                return RedirectToAction("Plans", "Subscription");
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
            {
                return NotFound();
            }

            // Increment view count
            movie.ViewsCount++;
            await _context.SaveChangesAsync();

            return View("~/Views/User/Movies/Watch.cshtml", movie);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
            {
                return NotFound();
            }

            // Get reviews
            var reviews = await _context.MovieReviews
                .Include(r => r.User)
                .Where(r => r.MovieId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // Calculate average rating
            double averageRating = 0;
            if (reviews.Any())
            {
                averageRating = reviews.Average(r => (double)r.Rating);
            }

            // Get similar movies
            var similarMovies = await _context.Movies
                .Where(m => m.IsActive && m.Genre == movie.Genre && m.Id != id)
                .OrderByDescending(m => m.Rating)
                .Take(6)
                .ToListAsync();

            // Check if user has active subscription
            var hasSubscription = await _context.UserSubscriptions
                .AnyAsync(s => s.UserId == userId && s.EndDate >= DateTime.Now && s.IsActive);

            // Create rating distribution
            var ratingDistribution = new int[10];
            foreach (var review in reviews)
            {
                if (review.Rating >= 1 && review.Rating <= 10)
                {
                    ratingDistribution[review.Rating - 1]++;
                }
            }

            var viewModel = new GuestMovieDetailsViewModel
            {
                Movie = movie,
                Reviews = reviews,
                SimilarMovies = similarMovies,
                RelatedMovies = similarMovies, // Use similar movies as related
                IsInWishlist = false, // You can implement wishlist check here
                AverageRating = averageRating,
                TotalReviews = reviews.Count,
                CanWatch = hasSubscription,
                RatingInfo = new MovieRatingViewModel
                {
                    MovieId = movie.Id,
                    MovieTitle = movie.Title ?? "Untitled",
                    AverageRating = averageRating,
                    TotalRatings = reviews.Count,
                    UserRating = 0,
                    RatingDistribution = ratingDistribution,
                    Reviews = reviews.Select(r => new MovieReviewViewModel
                    {
                        Id = r.Id,
                        MovieId = r.MovieId,
                        UserId = r.UserId,
                        UserName = r.User?.Name ?? "Anonymous",
                        UserProfilePicture = r.User?.ProfilePicture,
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        CreatedAt = r.CreatedAt,
                        HelpfulCount = r.HelpfulCount
                    }).ToList()
                }
            };

            return View("~/Views/User/Movies/Details.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int movieId, int rating, string reviewText)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return Json(new { success = false, message = "Movie not found" });
            }

            // Check if user already reviewed
            var existingReview = await _context.MovieReviews
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.ReviewText = reviewText;
                existingReview.UpdatedAt = DateTime.Now;
            }
            else
            {
                var review = new MovieReview
                {
                    MovieId = movieId,
                    UserId = userId,
                    Rating = rating,
                    ReviewText = reviewText,
                    CreatedAt = DateTime.Now,
                    HelpfulCount = 0
                };
                _context.MovieReviews.Add(review);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Review added successfully" });
        }
    }
}