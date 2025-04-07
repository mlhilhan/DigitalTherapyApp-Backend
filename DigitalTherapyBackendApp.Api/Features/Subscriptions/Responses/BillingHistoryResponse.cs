using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses
{
    public class BillingHistoryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<PaymentDto> Data { get; set; }
    }
}
