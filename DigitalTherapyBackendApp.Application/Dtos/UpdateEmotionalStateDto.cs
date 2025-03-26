using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos
{
    public class UpdateEmotionalStateDto
    {
        public int MoodLevel { get; set; }
        public List<string> Factors { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public bool IsBookmarked { get; set; }
    }
}
