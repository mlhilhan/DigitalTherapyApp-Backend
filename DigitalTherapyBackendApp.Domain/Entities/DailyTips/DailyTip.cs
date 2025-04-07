using System;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Domain.Entities.DailyTip
{
    public class DailyTip
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string TipKey { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBookmarked { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DailyTipCategory Category { get; set; }
        public ICollection<DailyTipTranslation> Translations { get; set; }
    }
}