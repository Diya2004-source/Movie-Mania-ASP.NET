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
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(150)]
        [Display(Name = "Referred User Email")]
        public string ReferredUserEmail { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Referral Code")]
        public string ReferralCode { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Expired

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Reward Amount")]
        public decimal RewardAmount { get; set; } = 0;

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Completed At")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedAt { get; set; }

        // Navigation Properties
        [ForeignKey("ReferrerId")]
        public virtual AppUser Referrer { get; set; }

        [ForeignKey("ReferredUserId")]
        public virtual AppUser ReferredUser { get; set; }
    }
}