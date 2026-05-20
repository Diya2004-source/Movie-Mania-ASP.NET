using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Referrals")]
    public class Referral
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReferrerId { get; set; }

        public int? ReferredUserId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string ReferredUserEmail { get; set; }

        [Required]
        [StringLength(20)]
        public string ReferralCode { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "decimal(18,2)")]
        public decimal RewardAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("ReferrerId")]
        public virtual AppUser Referrer { get; set; }

        [ForeignKey("ReferredUserId")]
        public virtual AppUser ReferredUser { get; set; }
    }
}