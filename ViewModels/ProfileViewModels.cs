using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProfilePictureUrl { get; set; }

        public UserSubscription CurrentSubscription { get; set; }
        public List<UserSubscription> SubscriptionHistory { get; set; } = new List<UserSubscription>();
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();

        public int TotalWishlistItems { get; set; }
        public int TotalMoviesWatched { get; set; }
        public int TotalEpisodesWatched { get; set; }
        public int WatchTimeMinutes { get; set; }

        public List<UserActivity> RecentlyWatched { get; set; } = new List<UserActivity>();
        public List<Wishlist> WishlistPreview { get; set; } = new List<Wishlist>();
    }

    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class ReferralViewModel
    {
        public string ReferralCode { get; set; }
        public int TotalReferrals { get; set; }
        public int RewardPoints { get; set; }
        public List<Referral> ReferralHistory { get; set; } = new List<Referral>();
        public int PendingReferrals { get; set; }
        public int CompletedReferrals { get; set; }
        public decimal TotalEarned { get; set; }
    }
}