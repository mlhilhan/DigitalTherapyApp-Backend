namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses
{
    public class EmotionalStateStatisticsResponse
    {
        public double AverageMoodIntensity { get; set; }
        public Dictionary<string, int> MoodFrequency { get; set; }
        public EmotionalStateResponse MostRecentMood { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
