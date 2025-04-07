using System;

namespace DigitalTherapyBackendApp.Application.Dtos.Subscription
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int UserSubscriptionId { get; set; }
        public UserSubscriptionDto UserSubscription { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDetails { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}