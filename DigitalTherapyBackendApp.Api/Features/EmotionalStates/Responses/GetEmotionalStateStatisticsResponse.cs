using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses
{
    public class GetEmotionalStateStatisticsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public EmotionalStateStatisticsDto Data { get; set; }
    }
}
