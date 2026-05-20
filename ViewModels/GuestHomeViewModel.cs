using System.Collections.Generic;

namespace MovieMania.ViewModels
{
    public class GuestHomeViewModel
    {
        public List<MovieViewModel> FeaturedMovies { get; set; } = new();
        public List<MovieViewModel> LatestMovies { get; set; } = new();
        public List<MovieViewModel> PopularMovies { get; set; } = new();
        public List<ShowViewModel> TrendingShows { get; set; } = new();
        public int TotalMovies { get; set; }
        public int TotalShows { get; set; }
        public int TotalUsers { get; set; }
        public List<MovieViewModel> RecommendedForYou { get; set; } = new();
    }
}