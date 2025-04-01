namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Responses
{
    public class ClearAiSessionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? SessionId { get; set; }
    }
}
