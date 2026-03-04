//using System;
//using System.ComponentModel.DataAnnotations;

//namespace MovieMania.Models
//{
//    public class Movie
//    {
//        public int Id { get; set; }

//        [Required]
//        public string Title { get; set; }

//        [Required]
//        public string Description { get; set; }

//        [Required]
//        public string Genre { get; set; }

//        [Required]
//        public int ReleaseYear { get; set; }

//        [Required]
//        public int Duration { get; set; }

//        public string Thumbnail { get; set; }

//        public string VideoUrl { get; set; }

//        public decimal Rating { get; set; } = 0;

//        public DateTime CreatedAt { get; set; } = DateTime.Now;
//    }
//}

using System;
using System.ComponentModel.DataAnnotations;

namespace MovieMania.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Release Year is required")]
        [Range(1900, 2100, ErrorMessage = "Enter a valid year")]
        public int ReleaseYear { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Video URL is required")]
        [Url(ErrorMessage = "Enter a valid URL")]
        public string VideoUrl { get; set; }

        public string Thumbnail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}