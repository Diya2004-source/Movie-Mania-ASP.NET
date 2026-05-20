using System;
using System.Collections.Generic;

namespace MovieMania.ViewModels
{
    public class ReferralViewModel
    {
        public string ReferralCode { get; set; } = string.Empty;
        public string ReferralLink { get; set; } = string.Empty;
        public int TotalReferrals { get; set; }
        public int PendingReferrals { get; set; }
        public int CompletedReferrals { get; set; }
        public decimal TotalRewards { get; set; }
        public decimal PendingRewards { get; set; }
        public List<ReferralDetailViewModel> ReferralHistory { get; set; } = new();
    }

    public class ReferralDetailViewModel
    {
        public int Id { get; set; }
        public string ReferredUserEmail { get; set; } = string.Empty;
        public string ReferredUserName { get; set; } = string.Empty;
        public DateTime ReferralDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Completed, Expired
        public decimal RewardAmount { get; set; }
    }

    public class ReferralStatsViewModel
    {
        public int TotalReferrals { get; set; }
        public int ActiveReferrals { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal PendingEarnings { get; set; }
        public string ReferralCode { get; set; } = string.Empty;
        public int ReferralRank { get; set; }
        public int TopReferrerCount { get; set; }
    }

    public class ReferralRequestViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}