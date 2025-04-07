using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class SubscriptionPlanResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public SubscriptionDetailsDto Data { get; set; }
    }
}
