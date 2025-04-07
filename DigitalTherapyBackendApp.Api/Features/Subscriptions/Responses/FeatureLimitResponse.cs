namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class FeatureLimitResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Limit { get; set; }
    }
}
