using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    // ==================== DASHBOARD VIEWMODELS ====================
    public class UserDashboardViewModel
    {
        public List<Movie> RecommendedMovies { get; set; } = new List<Movie>();
        public List<Movie> TrendingMovies { get; set; } = new List<Movie>();
        public List<Show> TrendingShows { get; set; } = new List<Show>();
        public List<Movie> AllMovies { get; set; } = new List<Movie>();        // Add this
        public List<Show> AllShows { get; set; } = new List<Show>();          // Add this
        public List<Show> AnimeShows { get; set; } = new List<Show>();        // Add this
        public List<Wishlist> UserWishlist { get; set; } = new List<Wishlist>();
        public List<UserActivity> ContinueWatching { get; set; } = new List<UserActivity>();
        public int TotalWishlistCount { get; set; }
        public int RecentlyWatchedCount { get; set; }
        public int TotalWatchTimeMinutes { get; set; }
        public UserSubscription ActiveSubscription { get; set; }
        public string UserName { get; set; }
    }

    // ==================== MOVIE DETAILS VIEWMODELS ====================
    public class UserMovieDetailsViewModel
    {
        public Movie Movie { get; set; } = new Movie();
        public bool IsInWishlist { get; set; }
        public decimal? UserRating { get; set; }
        public List<Movie> RelatedMovies { get; set; } = new List<Movie>();
        public List<int> RelatedMoviesInWishlist { get; set; } = new List<int>();
        public List<MovieReview> Reviews { get; set; } = new List<MovieReview>();
    }

    // ==================== SHOW DETAILS VIEWMODELS ====================
    public class UserShowDetailsViewModel
    {
        public Show Show { get; set; } = new Show();
        public bool IsInWishlist { get; set; }
        public Dictionary<int, List<Episode>> EpisodesBySeason { get; set; } = new Dictionary<int, List<Episode>>();
        public List<int> WatchedEpisodes { get; set; } = new List<int>();
        public int WatchProgress { get; set; }
        public List<Show> RelatedShows { get; set; } = new List<Show>();
        public List<int> RelatedShowsInWishlist { get; set; } = new List<int>();
        public List<ShowReview> Reviews { get; set; } = new List<ShowReview>();
    }

    // ==================== CATEGORIES VIEWMODELS ====================
    public class UserCategoriesViewModel
    {
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public Dictionary<string, List<Movie>> MoviesByGenre { get; set; } = new Dictionary<string, List<Movie>>();
        public Dictionary<string, List<Show>> ShowsByGenre { get; set; } = new Dictionary<string, List<Show>>();
        public List<string> PopularTags { get; set; } = new List<string>();
    }
}