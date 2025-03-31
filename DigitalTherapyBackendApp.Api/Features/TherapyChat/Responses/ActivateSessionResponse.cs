using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses
{
    public class ActivateSessionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ChatSessionDto Data { get; set; }
    }
}
