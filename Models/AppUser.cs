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

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Role")]
        public string Role { get; set; } = "user";

        [Display(Name = "Member Since")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Last Login")]
        [DataType(DataType.DateTime)]
        public DateTime? LastLoginAt { get; set; }

        [Display(Name = "Referral Code")]
        [StringLength(20)]
        public string? ReferralCode { get; set; }  // Made nullable

        [Display(Name = "Total Referrals")]
        public int TotalReferrals { get; set; } = 0;

        [Display(Name = "Reward Points")]
        public int RewardPoints { get; set; } = 0;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Profile Picture")]
        [StringLength(500)]
        [Url(ErrorMessage = "Invalid URL format")]
        public string? ProfilePictureUrl { get; set; }  // Made nullable

        // Navigation Properties
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public virtual ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Referral> ReferralsMade { get; set; } = new List<Referral>();
        public virtual ICollection<Referral> ReferralsReceived { get; set; } = new List<Referral>();
        public virtual ICollection<MovieReview> MovieReviews { get; set; } = new List<MovieReview>();
        public virtual ICollection<ShowReview> ShowReviews { get; set; } = new List<ShowReview>();
        public virtual ICollection<EpisodeComment> EpisodeComments { get; set; } = new List<EpisodeComment>();
        public virtual ICollection<UserActivity> Activities { get; set; } = new List<UserActivity>();
        public virtual ICollection<WishlistShow> WishlistShows { get; set; } = new List<WishlistShow>();
    }
}