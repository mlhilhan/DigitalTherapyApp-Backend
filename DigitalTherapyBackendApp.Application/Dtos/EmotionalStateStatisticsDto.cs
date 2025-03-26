using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos
{
    public class EmotionalStateStatisticsDto
    {
        public double AverageMood { get; set; }
        public Dictionary<string, int> FactorFrequency { get; set; }
        public List<DailyMoodDto> DailyMoods { get; set; }
        public int TotalEntries { get; set; }
    }
}
