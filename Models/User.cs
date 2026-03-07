using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMania.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
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

        // Navigation Properties
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

        public virtual ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public virtual ICollection<Referral> ReferralsMade { get; set; } = new List<Referral>();

        public virtual ICollection<Referral> ReferralsReceived { get; set; } = new List<Referral>();
    }
}