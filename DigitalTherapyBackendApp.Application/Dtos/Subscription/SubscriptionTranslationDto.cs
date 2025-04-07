using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos.Subscription
{
    public class SubscriptionTranslationDto
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string LanguageCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
