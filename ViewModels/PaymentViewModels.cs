using System.ComponentModel.DataAnnotations;

namespace MovieMania.ViewModels
{
    public class CheckoutViewModel
    {
        public int SubscriptionId { get; set; }
        public string PlanName { get; set; }
        public decimal Amount { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }

    public class PaymentProcessViewModel
    {
        [Required]
        public int SubscriptionId { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string CardCvv { get; set; }
        public string CardHolderName { get; set; }
        public string UpiId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string IfscCode { get; set; }
    }
}