using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses
{
    public class UpdatePsychologistSpecialtiesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Data { get; set; }
    }
}