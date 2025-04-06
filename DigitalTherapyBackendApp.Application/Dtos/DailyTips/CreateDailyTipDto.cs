using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Application.Dtos.DailyTips
{
    public class CreateDailyTipDto
    {
        public string TipKey { get; set; }
        public int CategoryId { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsFeatured { get; set; }

        public Dictionary<string, TipTranslationDto> Translations { get; set; }
    }

    public class TipTranslationDto
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
    }
}