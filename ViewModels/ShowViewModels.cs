using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class ShowDetailsViewModel
    {
        public Show Show { get; set; }
        public bool IsInWishlist { get; set; }
        public Dictionary<int, List<Episode>> EpisodesBySeason { get; set; }
        public List<int> WatchedEpisodes { get; set; }
        public int WatchProgress { get; set; }
        public List<Show> RelatedShows { get; set; }
    }
}