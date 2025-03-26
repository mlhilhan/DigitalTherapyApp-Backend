namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Payloads
{
    public class UpdateEmotionalStatePayload
    {
        public int MoodLevel { get; set; }
        public List<string>? Factors { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public bool IsBookmarked { get; set; }
    }
}
