using System.Collections.Generic;

namespace MovieMania.ViewModels
{
    public class MovieRatingViewModel
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int UserRating { get; set; }
        public int[] RatingDistribution { get; set; } = new int[10];
        public List<MovieReviewViewModel> Reviews { get; set; } = new();
    }
}