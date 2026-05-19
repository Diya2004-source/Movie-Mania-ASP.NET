using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("UserSubscriptions")]
    public class UserSubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PlanId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }  // Add this - make it nullable with ?

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }  // Add this - nullable

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }  // Make nullable

        [ForeignKey("PlanId")]
        public virtual SubscriptionPlan? SubscriptionPlan { get; set; }  // Make nullable
    }
}