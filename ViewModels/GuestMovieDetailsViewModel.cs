using System.Collections.Generic;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class GuestMovieDetailsViewModel
    {
        public Movie Movie { get; set; } = new();
        public List<MovieReview> Reviews { get; set; } = new();
        public List<Movie> SimilarMovies { get; set; } = new();
        public List<Movie> RelatedMovies { get; set; } = new();
        public bool IsInWishlist { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public bool CanWatch { get; set; }
        public MovieRatingViewModel? RatingInfo { get; set; }
    }
}