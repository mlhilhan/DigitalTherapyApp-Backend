using System;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class DailyTipCategory
    {
        public int Id { get; set; }
        public string CategoryKey { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<DailyTipCategoryTranslation> Translations { get; set; }
        public ICollection<DailyTip> Tips { get; set; }
    }
}