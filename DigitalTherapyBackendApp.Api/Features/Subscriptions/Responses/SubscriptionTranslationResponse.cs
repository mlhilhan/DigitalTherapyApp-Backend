using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class SubscriptionTranslationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public SubscriptionTranslationDto Data { get; set; }
    }
}
