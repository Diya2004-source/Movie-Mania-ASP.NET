using System;

namespace MovieMania.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }   // ✅ REQUIRED

        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }   // ✅ REQUIRED

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "active";
    }
}