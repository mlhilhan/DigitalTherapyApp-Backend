using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities.Subscriptions
{
    public class SubscriptionTranslation
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string LanguageCode { get; set; } // tr, en, de, vs.
        public string Name { get; set; }
        public string Description { get; set; }

        public Subscription Subscription { get; set; }
    }
}
