using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class EmotionalState
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int MoodLevel { get; set; }
        public List<string>? Factors { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsDeleted { get; set; }
    }
}
