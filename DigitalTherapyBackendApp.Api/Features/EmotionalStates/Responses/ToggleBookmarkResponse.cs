namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses
{
    public class ToggleBookmarkResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ErrorCode { get; set; }
        public bool? IsAdded { get; set; }
        public EmotionalStateData? Data { get; set; }
    }
}
