using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    // Guest movie details - NO UserMovieDetailsViewModel here
    public class GuestMovieDetailsViewModel
    {
        public Movie Movie { get; set; } = new Movie();
        public List<Movie> RelatedMovies { get; set; } = new List<Movie>();
        public List<MovieReview> Reviews { get; set; } = new List<MovieReview>();
    }

    // Movie rating (FIXES THE ERROR)
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