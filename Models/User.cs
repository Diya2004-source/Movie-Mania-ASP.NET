using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; } = "user";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ✅ Navigation Properties (ADD THESE BACK)
        public ICollection<Wishlist> Wishlists { get; set; }
        public ICollection<UserSubscription> Subscriptions { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Referral> ReferralsMade { get; set; }
        public ICollection<Referral> ReferralsReceived { get; set; }
    }
}