using System.Collections.Generic;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class UserDashboardViewModel
    {
        public List<Movie> RecommendedMovies { get; set; } = new List<Movie>();
        public List<Movie> TrendingMovies { get; set; } = new List<Movie>();
        public List<Show> TrendingShows { get; set; } = new List<Show>();
        public List<Wishlist> UserWishlist { get; set; } = new List<Wishlist>();
        public List<UserActivity> ContinueWatching { get; set; } = new List<UserActivity>();
        public int TotalWishlistCount { get; set; }
        public int RecentlyWatchedCount { get; set; }
        public int TotalWatchTimeMinutes { get; set; }
        public UserSubscription ActiveSubscription { get; set; }
        public string UserName { get; set; }
    }

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

    public class UserCategoriesViewModel
    {
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public Dictionary<string, List<Movie>> MoviesByGenre { get; set; } = new Dictionary<string, List<Movie>>();
        public Dictionary<string, List<Show>> ShowsByGenre { get; set; } = new Dictionary<string, List<Show>>();
        public List<string> PopularTags { get; set; } = new List<string>();
    }
}