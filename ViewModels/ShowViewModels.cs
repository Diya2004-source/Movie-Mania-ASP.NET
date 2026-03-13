using System.Collections.Generic;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    // Guest show details (if needed)
    public class ShowDetailsViewModel
    {
        public Show Show { get; set; } = new Show();
        public Dictionary<int, List<Episode>> EpisodesBySeason { get; set; } = new Dictionary<int, List<Episode>>();
        public List<Show> RelatedShows { get; set; } = new List<Show>();
        public List<ShowReview> Reviews { get; set; } = new List<ShowReview>();
    }
}