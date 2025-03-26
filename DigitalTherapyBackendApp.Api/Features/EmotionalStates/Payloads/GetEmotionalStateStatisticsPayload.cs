namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Payloads
{
    public class GetEmotionalStateStatisticsPayload
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
