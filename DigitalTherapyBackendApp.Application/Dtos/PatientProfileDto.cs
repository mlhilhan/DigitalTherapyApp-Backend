using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Dtos
{
    public class PatientProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string PreferredLanguage { get; set; }
        public string NotificationPreferences { get; set; }
        public string RelationshipStatus { get; set; }
        public DateTime? RelationshipStartDate { get; set; }
        public Guid? PsychologistId { get; set; }
        public string PsychologistName { get; set; }
    }
}
