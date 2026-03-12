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

        public int? SubscriptionPlanId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 9999.99, ErrorMessage = "Amount must be between 0.01 and 9999.99")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Date")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [StringLength(100)]
        [Display(Name = "Transaction ID")]
        public string? TransactionId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Payment Details")]
        public string? PaymentDetails { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }

        [ForeignKey("SubscriptionPlanId")]
        public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
    }
}