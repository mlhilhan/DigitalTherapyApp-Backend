using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses
{
    public class GetChatMessagesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ChatMessageDto> Data { get; set; }
    }
}
