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
        public List<UserSubscription> SubscriptionHistory { get; set; }
        public List<Payment> RecentPayments { get; set; }

        public int TotalWishlistItems { get; set; }
        public int TotalMoviesWatched { get; set; }
        public int TotalEpisodesWatched { get; set; }
        public int WatchTimeMinutes { get; set; }

        public List<UserActivity> RecentlyWatched { get; set; }
        public List<Wishlist> WishlistPreview { get; set; }
    }

    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2)]
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
        [StringLength(100, MinimumLength = 6)]
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
        public List<Referral> ReferralHistory { get; set; }
        public int PendingReferrals { get; set; }
        public int CompletedReferrals { get; set; }
        public decimal TotalEarned { get; set; }
    }
}