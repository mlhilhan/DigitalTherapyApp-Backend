using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Application.Dtos.DailyTips
{
    public class CreateDailyTipCategoryDto
    {
        public string CategoryKey { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }

        public Dictionary<string, string> Translations { get; set; }
    }
}