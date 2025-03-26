using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses
{
    public class UpdatePsychologistProfileResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PsychologistProfileDto Data { get; set; }
    }
}