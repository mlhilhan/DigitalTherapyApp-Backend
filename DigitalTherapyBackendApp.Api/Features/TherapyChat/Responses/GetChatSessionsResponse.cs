using DigitalTherapyBackendApp.Application.Dtos;

namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses
{
    public class GetChatSessionsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ChatSessionDto> Data { get; set; }
    }
}
