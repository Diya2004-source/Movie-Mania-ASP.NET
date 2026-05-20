using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("UserActivities")]
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? MovieId { get; set; }
        public int? EpisodeId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Activity Type")]
        public string ActivityType { get; set; } // Watch, Like, Comment, etc.

        [Display(Name = "Watch Duration (seconds)")]
        public int? WatchDuration { get; set; }

        [Display(Name = "Progress Percentage")]
        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;

        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; } = false;

        [Display(Name = "Last Position")]
        public TimeSpan? LastPosition { get; set; }

        [Display(Name = "Activity Date")]
        [DataType(DataType.DateTime)]
        public DateTime ActivityDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        [ForeignKey("EpisodeId")]
        public virtual Episode Episode { get; set; }
    }
}