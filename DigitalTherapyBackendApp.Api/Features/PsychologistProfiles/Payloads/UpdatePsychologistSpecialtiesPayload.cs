using System.ComponentModel.DataAnnotations;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Payloads
{
    public class UpdatePsychologistSpecialtiesPayload
    {
        [Required]
        public List<string> Specialties { get; set; }
    }
}
