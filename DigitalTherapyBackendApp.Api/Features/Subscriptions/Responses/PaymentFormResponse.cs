using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class PaymentFormResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PaymentFormDto Data { get; set; }
    }
}
