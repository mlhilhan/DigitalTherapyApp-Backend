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
        public Guid? TherapistId { get; set; } // Null olabilir (AI destekli oturumlar için)
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Summary { get; set; }
        public string Status { get; set; } // Scheduled, InProgress, Completed
        public User Patient { get; set; }
        public User Therapist { get; set; }
        public ICollection<SessionMessage> Messages { get; set; }
    }
}
