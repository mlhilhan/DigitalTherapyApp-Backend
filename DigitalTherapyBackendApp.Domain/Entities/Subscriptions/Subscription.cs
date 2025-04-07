using DigitalTherapyBackendApp.Domain.Entities.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities.Subscriptions
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; } // USD cinsinden temel fiyat
        public string BaseCurrency { get; set; } = "USD"; // Temel para birimi
        public string PlanId { get; set; } // Örn: "free", "standard", "premium", "pro"
        public int DurationInDays { get; set; } // 30, 90, 365 gün
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

        public ICollection<UserSubscription> UserSubscriptions { get; set; }
        public ICollection<SubscriptionPrice> PricesByCountry { get; set; }
        public ICollection<SubscriptionTranslation> Translations { get; set; }
    }
}
