using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }  // Make nullable

        public DateTime PaymentDate { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }  // Make nullable

        [StringLength(100)]
        public string? TransactionId { get; set; }  // Make nullable

        [StringLength(500)]
        public string? PaymentDetails { get; set; }  // ADD THIS - for admin details view

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }  // Make nullable

        [ForeignKey("SubscriptionPlanId")]
        public virtual SubscriptionPlan? SubscriptionPlan { get; set; }  // Make nullable
    }
}