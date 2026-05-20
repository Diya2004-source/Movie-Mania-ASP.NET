using System;
using MovieMania.Models;

namespace MovieMania.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? ProfilePicture { get; set; }
        public UserSubscription? ActiveSubscription { get; set; }
        public string? ReferralCode { get; set; }
        public int TotalReferrals { get; set; }
        public int RewardPoints { get; set; }
    }
}