using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("SubscriptionPlans")]
    public class SubscriptionPlan
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Plan name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Plan name must be between 2 and 100 characters")]
        [Display(Name = "Plan Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000, ErrorMessage = "Price must be between 0 and 1000")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Duration (days)")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int DurationInDays { get; set; } = 30;  // Changed from DurationDays

        [Display(Name = "Features")]
        public string Features { get; set; }

        [Display(Name = "Max Screens")]
        [Range(1, 10, ErrorMessage = "Max screens must be between 1 and 10")]
        public int MaxScreens { get; set; } = 1;

        [Display(Name = "Video Quality")]
        [StringLength(20)]
        public string VideoQuality { get; set; } = "HD";

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;  // Changed from Status

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }
    }
}