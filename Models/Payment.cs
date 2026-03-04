using System;

namespace MovieMania.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }   // ✅ REQUIRED

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}