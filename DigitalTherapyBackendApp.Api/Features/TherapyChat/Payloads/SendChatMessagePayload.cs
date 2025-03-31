namespace DigitalTherapyBackendApp.Api.Features.TherapyChat.Payloads
{
    public class SendChatMessagePayload
    {
        public string Message { get; set; }
        public Guid? SessionId { get; set; }
    }
}
