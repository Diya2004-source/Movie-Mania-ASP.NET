<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
﻿//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MovieMania.Models
//{
//    [Table("Episodes")]
//    public class Episode
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required(ErrorMessage = "Episode title is required")]
//        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
//        [Display(Name = "Episode Title")]
//        public string Title { get; set; }

//        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
//        [DataType(DataType.MultilineText)]
//        [Display(Name = "Description")]
//        public string Description { get; set; }

//        [Required]
//        [Display(Name = "Show")]
//        public int ShowId { get; set; }

//        [Required(ErrorMessage = "Season number is required")]
//        [Display(Name = "Season Number")]
//        [Range(1, 50, ErrorMessage = "Season number must be between 1 and 50")]
//        public int SeasonNumber { get; set; }

//        [Required(ErrorMessage = "Episode number is required")]
//        [Display(Name = "Episode Number")]
//        [Range(1, 100, ErrorMessage = "Episode number must be between 1 and 100")]
//        public int EpisodeNumber { get; set; }

//        [Display(Name = "Duration (minutes)")]
//        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes")]
//        public int? Duration { get; set; }

//        [Required(ErrorMessage = "Video URL is required")]
//        [Display(Name = "Video URL")]
//        [Url(ErrorMessage = "Please enter a valid URL")]
//        [StringLength(500)]
//        public string VideoUrl { get; set; }

//        [Display(Name = "Thumbnail URL")]
//        [Url(ErrorMessage = "Please enter a valid URL")]
//        [StringLength(500)]
//        public string ThumbnailUrl { get; set; }

//        [DataType(DataType.Date)]
//        [Display(Name = "Release Date")]
//        public DateTime? ReleaseDate { get; set; }

//        [Display(Name = "Is Active")]
//        public bool IsActive { get; set; } = true;

//        [Display(Name = "Views Count")]
//        public int ViewsCount { get; set; } = 0;

//        [Display(Name = "Created At")]
//        [DataType(DataType.DateTime)]
//        public DateTime CreatedAt { get; set; } = DateTime.Now;

//        [Display(Name = "Updated At")]
//        [DataType(DataType.DateTime)]
//        public DateTime? UpdatedAt { get; set; }

//        // Navigation Properties
//        [ForeignKey("ShowId")]
//        public virtual Show Show { get; set; }

//        public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
//        public virtual ICollection<EpisodeComment> Comments { get; set; } = new List<EpisodeComment>();
//    }
//}

using System;
<<<<<<< HEAD
=======
=======
﻿using System;
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Episodes")]
    public class Episode
    {
        [Key]
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Episode title is required")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "Title must be between 2 and 200 characters")]
        [Display(Name = "Episode Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000,
            ErrorMessage = "Description cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
<<<<<<< HEAD
=======
=======
        public int Id { get; set; }

        [Required(ErrorMessage = "Episode title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        [Display(Name = "Episode Title")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3

        [Required]
        [Display(Name = "Show")]
        public int ShowId { get; set; }

        [Required(ErrorMessage = "Season number is required")]
        [Display(Name = "Season Number")]
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        [Range(1, 50,
            ErrorMessage = "Season number must be between 1 and 50")]
        public int SeasonNumber { get; set; } = 1;

        [Required(ErrorMessage = "Episode number is required")]
        [Display(Name = "Episode Number")]
        [Range(1, 100,
            ErrorMessage = "Episode number must be between 1 and 100")]
        public int EpisodeNumber { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(1, 300,
            ErrorMessage = "Duration must be between 1 and 300 minutes")]
<<<<<<< HEAD
=======
=======
        [Range(1, 50, ErrorMessage = "Season number must be between 1 and 50")]
        public int SeasonNumber { get; set; }

        [Required(ErrorMessage = "Episode number is required")]
        [Display(Name = "Episode Number")]
        [Range(1, 100, ErrorMessage = "Episode number must be between 1 and 100")]
        public int EpisodeNumber { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes")]
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        public int? Duration { get; set; }

        [Required(ErrorMessage = "Video URL is required")]
        [Display(Name = "Video URL")]
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        [StringLength(500)]
        public string VideoUrl { get; set; } = string.Empty;

        [Display(Name = "Thumbnail URL")]
        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }
<<<<<<< HEAD
=======
=======
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string VideoUrl { get; set; }

        [Display(Name = "Thumbnail URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string ThumbnailUrl { get; set; }
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Views Count")]
        public int ViewsCount { get; set; } = 0;

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
        // Navigation Property
        [ForeignKey("ShowId")]
        public virtual Show? Show { get; set; }

        public virtual ICollection<UserActivity> UserActivities { get; set; }
            = new List<UserActivity>();

        public virtual ICollection<EpisodeComment> Comments { get; set; }
            = new List<EpisodeComment>();
<<<<<<< HEAD
=======
=======
        // Navigation Properties to navigate
        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; }

        public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
        public virtual ICollection<EpisodeComment> Comments { get; set; } = new List<EpisodeComment>();
>>>>>>> 14ac8531bd2a898f14d4c39038b54eaa701e3c1d
>>>>>>> e6456616c907c0f5683d34071f3b0624a05bc2d3
    }
}