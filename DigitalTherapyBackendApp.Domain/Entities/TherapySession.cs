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
        public Guid PatientId { get; set; }
        public Guid? PsychologistId { get; set; } // Null olabilir (AI destekli oturumlar için)
        public Guid? RelationshipId { get; set; } // Hasta-Psikolog ilişkisi (AI oturumları için null)
        public bool IsAiSession { get; set; } // AI ile yapılan bir seans mı?
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Summary { get; set; }
        public string Status { get; set; } // Scheduled, InProgress, Completed
        public string SessionType { get; set; } // Video, Audio, Text
        public string MeetingLink { get; set; } // Video/ses görüşmeleri için link
        public User Patient { get; set; }
        public User Therapist { get; set; }
        public TherapistPatientRelationship Relationship { get; set; }
        public ICollection<SessionMessage> Messages { get; set; } = new List<SessionMessage>();
        public ICollection<EmotionalState> EmotionalStates { get; set; } = new List<EmotionalState>();
    }
}
