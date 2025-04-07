using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos.Subscription
{
    public class SubscriptionDetailsDto
    {
        public int Id { get; set; }
        public string PlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public int DurationInDays { get; set; }
        public int MoodEntryLimit { get; set; }
        public int AIChatSessionsPerWeek { get; set; }
        public int MessageLimitPerChat { get; set; }
        public bool HasPsychologistSupport { get; set; }
        public int PsychologistSessionsPerMonth { get; set; }
        public bool HasEmergencySupport { get; set; }
        public bool HasAdvancedReports { get; set; }
        public bool HasAllMeditationContent { get; set; }
    }
}
