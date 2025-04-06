using System;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class DailyTipTranslation
    {
        public int Id { get; set; }
        public int TipId { get; set; }
        public string LanguageCode { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DailyTip Tip { get; set; }
    }
}