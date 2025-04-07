using System;

namespace DigitalTherapyBackendApp.Domain.Entities.DailyTip
{
    public class DailyTipCategoryTranslation
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string LanguageCode { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DailyTipCategory Category { get; set; }
    }
}