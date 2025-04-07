using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities.Subscriptions
{
    public class SubscriptionPrice
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string CountryCode { get; set; } // TR, US, DE, vs.
        public string CurrencyCode { get; set; } // TRY, USD, EUR, vs.
        public decimal Price { get; set; }

        public Subscription Subscription { get; set; }
    }
}
