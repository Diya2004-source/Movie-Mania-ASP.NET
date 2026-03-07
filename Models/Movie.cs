using System;
using System.ComponentModel.DataAnnotations;

namespace MovieMania.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int? ReleaseYear { get; set; }

        public int? Duration { get; set; }

        public decimal? Rating { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Thumbnail { get; set; }

        public string VideoUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}