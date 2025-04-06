using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Application.Dtos.DailyTips
{
    public class DailyTipCategoryDto
    {
        public int Id { get; set; }
        public string CategoryKey { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public List<DailyTipDto> Tips { get; set; }
    }
}