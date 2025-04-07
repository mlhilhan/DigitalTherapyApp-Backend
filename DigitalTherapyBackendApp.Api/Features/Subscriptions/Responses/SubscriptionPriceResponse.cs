using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class SubscriptionPriceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public SubscriptionPriceDto Data { get; set; }
    }
}
