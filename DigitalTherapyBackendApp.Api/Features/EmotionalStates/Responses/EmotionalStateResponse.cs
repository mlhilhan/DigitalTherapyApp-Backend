namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses
{
    public class EmotionalStateResponse
    {
        public Guid Id { get; set; }
        public string Mood { get; set; }
        public int MoodIntensity { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
