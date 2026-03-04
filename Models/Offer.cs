using System;
using System.ComponentModel.DataAnnotations;

namespace MovieMania.Models
{
    public class Offer
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal DiscountPercentage { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}