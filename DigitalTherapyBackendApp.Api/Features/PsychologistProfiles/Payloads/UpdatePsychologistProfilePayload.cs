using System.ComponentModel.DataAnnotations;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Payloads
{
    public class UpdatePsychologistProfilePayload
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; }

        public string Bio { get; set; }

        public string PreferredLanguage { get; set; }

        public string NotificationPreferences { get; set; }

        public Guid? InstitutionId { get; set; }

        public string Education { get; set; }

        public string Certifications { get; set; }

        public string Experience { get; set; }

        public string LicenseNumber { get; set; }
    }
}