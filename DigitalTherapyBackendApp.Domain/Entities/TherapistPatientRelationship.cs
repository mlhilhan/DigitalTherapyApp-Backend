using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class TherapistPatientRelationship
    {
        public Guid Id { get; set; }
        public Guid PsychologistId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } // Active, Pending, Completed, Terminated
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PsychologistProfile Psychologist { get; set; }
        public PatientProfile Patient { get; set; }

        public bool IsActive()
        {
            return Status == "Active" && (EndDate == null || EndDate > DateTime.UtcNow);
        }
    }
}
