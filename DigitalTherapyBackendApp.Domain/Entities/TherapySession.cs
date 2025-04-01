using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class TherapySession
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; } // Hasta kullanıcı ID'si
        public Guid? PsychologistId { get; set; } // Psikolog ID'si (AI için null)
        public Guid? RelationshipId { get; set; } // Hasta-Psikolog ilişkisi (AI oturumları için null)
        public bool IsAiSession { get; set; } // AI oturumu mu?
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? ReactivatedAt { get; set; } // Oturumun yeniden aktifleştirildiği tarih/saat
        public SessionStatus Status { get; set; } // Enum: Scheduled, InProgress, Completed, Cancelled
        public SessionType Type { get; set; } // Enum: Text, Video, Voice
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public string? MeetingLink { get; set; } // Video/ses görüşmeleri için link

        // Navigation properties
        public virtual PatientProfile Patient { get; set; }
        public virtual PsychologistProfile Psychologist { get; set; }
        public virtual ICollection<SessionMessage> Messages { get; set; } = new List<SessionMessage>();
        public TherapistPatientRelationship Relationship { get; set; }
        public ICollection<EmotionalState> EmotionalStates { get; set; } = new List<EmotionalState>();
    }


    public enum SessionStatus
    {
        Scheduled,  // Randevu alındı, henüz başlamadı
        InProgress, // Seans devam ediyor
        Completed,  // Seans tamamlandı
        Cancelled,   // Seans iptal edildi
        Archived    // Seans arşivlendi
    }

    public enum SessionType
    {
        Text,  // Yazılı mesajlaşma
        Video, // Görüntülü görüşme
        Voice  // Sesli görüşme
    }
}
