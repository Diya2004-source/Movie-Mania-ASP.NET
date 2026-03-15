using System.Collections.Generic;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class GuestShowDetailsViewModel
    {
        public Show Show { get; set; } = new();
        public List<Episode> Episodes { get; set; } = new();
        public List<ShowReview> Reviews { get; set; } = new();
        public List<Show> SimilarShows { get; set; } = new();
        public List<Show> RelatedShows { get; set; } = new();
        public bool IsInWishlist { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int TotalSeasons { get; set; }
        public Dictionary<int, List<Episode>> EpisodesBySeason { get; set; } = new();
    }
}