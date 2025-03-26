namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses
{
    public class EmotionalStateData
    {
        public Guid Id { get; set; }
        public int MoodLevel { get; set; }
        public List<string> Factors { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
