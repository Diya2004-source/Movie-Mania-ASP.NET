using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Movies")]
    public class Movie
    {
        [Key]
        public int Id { get; set; }

<<<<<<< HEAD
        [Required(ErrorMessage = "Movie title is required")]
=======
        [Required(ErrorMessage = "Movie title is require")]
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        [Display(Name = "Movie Title")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters")]
        [Display(Name = "Genre")]
        public string? Genre { get; set; }

        [Display(Name = "Genre")]
        public int? GenreId { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre? GenreNavigation { get; set; }  // Made nullable

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Release Year")]
        [Range(1900, 2100, ErrorMessage = "Release year must be between 1900 and 2100")]
        public int? ReleaseYear { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes")]
        public int? Duration { get; set; }

        [Display(Name = "Rating")]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public decimal? Rating { get; set; }

        [Display(Name = "Thumbnail URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        // Backward compatibility property
        [NotMapped]
        public string? Thumbnail
        {
            get { return ThumbnailUrl; }
            set { ThumbnailUrl = value; }
        }

        [Display(Name = "Video URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string? VideoUrl { get; set; }

        [Display(Name = "Trailer URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string? TrailerUrl { get; set; }

        [Display(Name = "Cast")]
        [StringLength(500)]
        public string? Cast { get; set; }

        [Display(Name = "Director")]
        [StringLength(200)]
        public string? Director { get; set; }

        [Display(Name = "Language")]
        [StringLength(50)]
        public string? Language { get; set; }

        [Display(Name = "Country")]
        [StringLength(100)]
        public string? Country { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Views Count")]
        public int ViewsCount { get; set; } = 0;

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<MovieReview>? Reviews { get; set; } = new List<MovieReview>();
        public virtual ICollection<UserActivity>? UserActivities { get; set; } = new List<UserActivity>();
    }
}