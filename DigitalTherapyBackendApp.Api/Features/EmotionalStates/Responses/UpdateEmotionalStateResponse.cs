namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses
{
    public class UpdateEmotionalStateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public EmotionalStateData? Data { get; set; }
    }
}
