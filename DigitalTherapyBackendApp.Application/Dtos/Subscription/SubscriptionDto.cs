using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos.Subscription
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string BaseCurrency { get; set; }
        public string PlanId { get; set; }
        public int DurationInDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int MoodEntryLimit { get; set; }
        public int AIChatSessionsPerWeek { get; set; }
        public int MessageLimitPerChat { get; set; }
        public bool HasPsychologistSupport { get; set; }
        public int PsychologistSessionsPerMonth { get; set; }
        public bool HasEmergencySupport { get; set; }
        public bool HasAdvancedReports { get; set; }
        public bool HasAllMeditationContent { get; set; }
        public List<SubscriptionPriceDto> PricesByCountry { get; set; }
        public List<SubscriptionTranslationDto> Translations { get; set; }
    }
}