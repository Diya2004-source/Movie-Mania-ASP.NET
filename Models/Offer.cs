using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Offers")]
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Offer title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        [Display(Name = "Offer Title")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Discount Percentage")]
        [Range(1, 100, ErrorMessage = "Discount must be between 1% and 100%")]
        public int DiscountPercentage { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Applicable Plan ID")]
        public int? SubscriptionPlanId { get; set; }

        [StringLength(50)]
        [Display(Name = "Offer Code")]
        public string OfferCode { get; set; }

        [Display(Name = "Max Uses")]
        public int? MaxUses { get; set; }

        [Display(Name = "Current Uses")]
        public int CurrentUses { get; set; } = 0;

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("SubscriptionPlanId")]
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }
}