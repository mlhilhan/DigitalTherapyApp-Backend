using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities.Subscriptions
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserSubscriptionId { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } // "completed", "pending", "failed"
        public string PaymentMethod { get; set; }
        public string PaymentDetails { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserSubscription UserSubscription { get; set; }
    }
}
