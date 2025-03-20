namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Payloads
{
    public class UpdateEmotionalStatePayload
    {
        public string Mood { get; set; }
        public int MoodIntensity { get; set; }
        public string Notes { get; set; }
    }
}
