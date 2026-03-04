using System.ComponentModel.DataAnnotations;

namespace MovieMania.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 3650, ErrorMessage = "Duration must be between 1 and 3650 days")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("active|inactive", ErrorMessage = "Status must be active or inactive")]
        public string Status { get; set; } = "active";
    }
}