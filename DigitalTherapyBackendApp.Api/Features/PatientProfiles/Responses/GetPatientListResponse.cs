using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.PatientProfiles.Responses
{
    public class GetPatientListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<PatientProfileDto> Data { get; set; }
    }
}
