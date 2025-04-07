using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos.Subscription
{
    public class SubscriptionPriceDto
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Price { get; set; }
    }
}
