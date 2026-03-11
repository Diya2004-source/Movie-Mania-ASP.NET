// ViewModels/MovieViewModels.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MovieMania.Models;  // Make sure this using is present

namespace MovieMania.ViewModels
{
    public class UserDashboardViewModel
    {
        public List<Movie> FeaturedMovies { get; set; } = new List<Movie>();
        public List<Movie> RecentMovies { get; set; } = new List<Movie>();
        public List<Movie> RecommendedMovies { get; set; } = new List<Movie>();
        public List<Show> TrendingShows { get; set; } = new List<Show>();
        public List<Wishlist> UserWishlist { get; set; } = new List<Wishlist>();
        public List<UserActivity> ContinueWatching { get; set; } = new List<UserActivity>();
        public int TotalWishlistCount { get; set; }
        public int RecentlyWatchedCount { get; set; }
        public UserSubscription ActiveSubscription { get; set; }
    }

    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; } = new Movie();
        public bool IsInWishlist { get; set; }
        public decimal? UserRating { get; set; }
        public List<Movie> RelatedMovies { get; set; } = new List<Movie>();
        public List<MovieReview> Reviews { get; set; } = new List<MovieReview>();
    }

    public class MovieRatingViewModel
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public decimal Rating { get; set; }

        [StringLength(500, ErrorMessage = "Review cannot exceed 500 characters")]
        public string Review { get; set; }
    }
}