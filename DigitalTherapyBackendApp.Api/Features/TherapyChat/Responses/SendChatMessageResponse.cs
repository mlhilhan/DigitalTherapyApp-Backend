using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses
{
    public class SendChatMessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ChatResponseDto Data { get; set; }
    }
}
