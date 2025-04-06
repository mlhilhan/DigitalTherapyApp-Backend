using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos.DailyTips
{
    public class UpdateDailyTipDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsFeatured { get; set; }

        public Dictionary<string, TipTranslationDto> Translations { get; set; }
    }
}
