using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;
using MovieMania.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Security.Claims;

namespace MovieMania.Controllers.Guest
{
    [Route("Movies")]
    public class GuestMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? genre, string? sortBy, int page = 1)
        {
            var query = _context.Movies.Where(m => m.IsActive);

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(m => m.Genre != null && m.Genre == genre);
            }

            int totalCount = await query.CountAsync();

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
                .ToListAsync(); // Return Movie entities, not ViewModels

            var genres = await _context.Movies
                .Where(m => m.IsActive && m.Genre != null)
                .Select(m => m.Genre!)
                .Distinct()
                .ToListAsync();

            ViewBag.Genres = genres;
            ViewBag.CurrentGenre = genre ?? "All";
            ViewBag.CurrentSort = sortBy ?? "latest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / 12) : 1;

            return View("~/Views/Guest/Home/Movies.cshtml", movies); // Pass List<Movie>
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (movie == null)
            {
                return NotFound();
            }

            movie.ViewsCount++;
            await _context.SaveChangesAsync();

            var reviews = await _context.MovieReviews
                .Include(r => r.User)
                .Where(r => r.MovieId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            double averageRating = 0;
            if (reviews != null && reviews.Any())
            {
                averageRating = reviews.Average(r => (double)r.Rating);
            }

            var similarMovies = await _context.Movies
                .Where(m => m.IsActive && m.Genre == movie.Genre && m.Id != id)
                .OrderByDescending(m => m.Rating)
                .Take(6)
                .ToListAsync();

            var relatedMovies = await _context.Movies
                .Where(m => m.IsActive && m.Id != id)
                .OrderByDescending(m => m.ViewsCount)
                .Take(6)
                .ToListAsync();

            var ratingDistribution = new int[10];
            if (reviews != null)
            {
                foreach (var review in reviews)
                {
                    if (review.Rating >= 1 && review.Rating <= 10)
                    {
                        ratingDistribution[review.Rating - 1]++;
                    }
                }
            }

            var viewModel = new GuestMovieDetailsViewModel
            {
                Movie = movie,
                Reviews = reviews ?? new List<MovieReview>(),
                SimilarMovies = similarMovies ?? new List<Movie>(),
                RelatedMovies = relatedMovies ?? new List<Movie>(),
                IsInWishlist = false,
                AverageRating = averageRating,
                TotalReviews = reviews?.Count ?? 0,
                CanWatch = false,
                RatingInfo = new MovieRatingViewModel
                {
                    MovieId = movie.Id,
                    MovieTitle = movie.Title ?? "Untitled",
                    AverageRating = averageRating,
                    TotalRatings = reviews?.Count ?? 0,
                    UserRating = 0,
                    RatingDistribution = ratingDistribution,
                    Reviews = reviews?.Select(r => new MovieReviewViewModel
                    {
                        Id = r.Id,
                        MovieId = r.MovieId,
                        UserId = r.UserId,
                        UserName = r.User?.Name ?? "Anonymous",
                        UserProfilePicture = r.User?.ProfilePicture,
                        Rating = r.Rating,
                        ReviewText = r.ReviewText ?? string.Empty,
                        CreatedAt = r.CreatedAt,
                        HelpfulCount = r.HelpfulCount,
                        IsHelpful = false
                    }).ToList() ?? new List<MovieReviewViewModel>()
                }
            };

            return View("~/Views/Guest/Home/MovieDetails.cshtml", viewModel);
        }
    }
}