using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Shows")]
    public class Show
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Show title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        [Display(Name = "Show Title")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters")]
        [Display(Name = "Genre")]
        public string Genre { get; set; }

        [Display(Name = "Genre")]
        public int? GenreId { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre GenreNavigation { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Release Year")]
        [Range(1900, 2100, ErrorMessage = "Release year must be between 1900 and 2100")]
        public int? ReleaseYear { get; set; }

        [Required(ErrorMessage = "Total seasons is required")]
        [Display(Name = "Total Seasons")]
        [Range(1, 50, ErrorMessage = "Total seasons must be between 1 and 50")]
        public int TotalSeasons { get; set; } = 1;

        [Required(ErrorMessage = "Total episodes is required")]
        [Display(Name = "Total Episodes")]
        [Range(1, 1000, ErrorMessage = "Total episodes must be between 1 and 1000")]
        public int TotalEpisodes { get; set; } = 0;

        [Display(Name = "Rating")]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public decimal? Rating { get; set; }

        [Display(Name = "Thumbnail URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string ThumbnailUrl { get; set; }

        [Display(Name = "Poster URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string PosterUrl { get; set; }

        [Display(Name = "Trailer URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string TrailerUrl { get; set; }

        [Display(Name = "Director")]
        [StringLength(200)]
        public string Director { get; set; }

        [Display(Name = "Cast")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Cast { get; set; }

        [Display(Name = "Language")]
        [StringLength(50)]
        public string Language { get; set; }

        [Display(Name = "Country")]
        [StringLength(100)]
        public string Country { get; set; }

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
        public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();
        public virtual ICollection<ShowReview> Reviews { get; set; } = new List<ShowReview>();
        public virtual ICollection<WishlistShow> WishlistShows { get; set; } = new List<WishlistShow>();
    }
}