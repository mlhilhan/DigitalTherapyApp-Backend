using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class SubscriptionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserSubscriptionDto Data { get; set; }
    }
}
