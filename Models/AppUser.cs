using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Users")]
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Role { get; set; } = "user";

        [StringLength(500)]
        public string? ProfilePicture { get; set; }  // This is the correct property name (not ProfilePictureUrl)

        // Computed property for view compatibility
        [NotMapped]
        public string? ProfilePictureUrl
        {
            get { return ProfilePicture; }
            set { ProfilePicture = value; }
        }

        public bool IsActive { get; set; } = true;

        // Referral and Rewards System
        public int TotalReferrals { get; set; } = 0;  // Add this
        public int RewardPoints { get; set; } = 0;    // Add this
        public string? ReferralCode { get; set; }     // Add this

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? LastLoginAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Wishlist>? Wishlists { get; set; }
        public virtual ICollection<UserSubscription>? Subscriptions { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<Referral>? ReferralsMade { get; set; }
        public virtual ICollection<Referral>? ReferralsReceived { get; set; }
        public virtual ICollection<MovieReview>? MovieReviews { get; set; }
        public virtual ICollection<ShowReview>? ShowReviews { get; set; }
        public virtual ICollection<EpisodeComment>? EpisodeComments { get; set; }
        public virtual ICollection<WishlistShow>? WishlistShows { get; set; }
        public virtual ICollection<UserActivity>? Activities { get; set; }
    }
}