using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.PatientProfiles.Responses
{
    public class GetPatientProfileResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PatientProfileDto Data { get; set; }
    }
}
