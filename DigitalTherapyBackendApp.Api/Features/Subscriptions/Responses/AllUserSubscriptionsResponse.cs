using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class AllUserSubscriptionsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<UserSubscriptionDto> Data { get; set; }
    }
}
