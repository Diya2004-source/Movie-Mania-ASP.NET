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
        public int SubscriptionPlanId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Active, Cancelled, Expired

        [StringLength(20)]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed

        [StringLength(100)]
        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }

        [Display(Name = "Cancelled At")]
        [DataType(DataType.DateTime)]
        public DateTime? CancelledAt { get; set; }

        [Display(Name = "Reactivated At")]
        [DataType(DataType.DateTime)]
        public DateTime? ReactivatedAt { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [ForeignKey("SubscriptionPlanId")]
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }
}