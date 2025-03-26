using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos
{
    public class DailyMoodDto
    {
        public DateTime Date { get; set; }
        public double AverageMood { get; set; }
        public int EntryCount { get; set; }
    }
}
