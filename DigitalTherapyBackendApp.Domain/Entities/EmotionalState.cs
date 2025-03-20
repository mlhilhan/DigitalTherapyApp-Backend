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
        public string Mood { get; set; } // Mutlu, Üzgün, Endişeli, Sakin vb.
        public int MoodIntensity { get; set; } // 1-10 arasında şiddet
        public string Notes { get; set; } // Kullanıcının açıklaması
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
    }
}
