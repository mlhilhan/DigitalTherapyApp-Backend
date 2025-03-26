using DigitalTherapyBackendApp.Application.Dtos;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses
{
    public class GetAvailablePsychologistsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<PsychologistProfileDto> Data { get; set; }
    }
}