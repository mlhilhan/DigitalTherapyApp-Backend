using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos.DailyTips
{
    public class UpdateDailyTipCategoryDto
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }

        public Dictionary<string, string> Translations { get; set; }
    }
}
