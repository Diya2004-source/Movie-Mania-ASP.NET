using System.ComponentModel.DataAnnotations;

namespace MovieMania.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

    public class ProcessPaymentViewModel
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        // Card details
        public string? CardNumber { get; set; }
        public string? CardExpiry { get; set; }
        public string? CardCvv { get; set; }
        public string? CardHolderName { get; set; }

        // UPI details
        public string? UpiId { get; set; }

        // NetBanking details
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IfscCode { get; set; }
    }
}