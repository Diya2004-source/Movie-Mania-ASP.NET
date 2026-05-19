using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Wishlist")]
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? MovieId { get; set; }
        public int? ShowId { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Item Type")]
        public string ItemType { get; set; } // "Movie" or "Show"

        [Display(Name = "Added Date")]
        [DataType(DataType.DateTime)]
        public DateTime AddedDate { get; set; } = DateTime.Now;

        [Display(Name = "Updated Date")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Priority")]
        [Range(1, 3, ErrorMessage = "Priority must be 1 (High), 2 (Medium), or 3 (Low)")]
        public int Priority { get; set; } = 1;

        [Display(Name = "Is Watched")]
        public bool IsWatched { get; set; } = false;

        [Display(Name = "Watched Date")]
        [DataType(DataType.DateTime)]
        public DateTime? WatchedDate { get; set; }

        [Display(Name = "User Rating")]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public decimal? UserRating { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Notification Enabled")]
        public bool NotificationEnabled { get; set; } = true;

        [Display(Name = "Watched Seasons")]
        public int? WatchedSeasons { get; set; }

        [Display(Name = "Watched Episodes")]
        public int? WatchedEpisodes { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; }
    }
}