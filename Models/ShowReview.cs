using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("ShowReviews")]
    public class ShowReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShowId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public decimal Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Review cannot exceed 1000 characters")]
        public string ReviewText { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;
        public int HelpfulCount { get; set; } = 0;

        // Navigation Properties - Using AppUser
        [ForeignKey("ShowId")]
        public virtual Show Show { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }  // Fixed: AppUser instead of User
    }
}