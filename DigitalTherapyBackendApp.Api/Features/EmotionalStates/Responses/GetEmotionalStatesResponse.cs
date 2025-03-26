using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses
{
    public class GetEmotionalStatesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public List<EmotionalStateDto> Data { get; set; }
    }
}
