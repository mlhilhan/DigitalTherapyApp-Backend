using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Queries
{
    public class GetBillingHistoryQuery : IRequest<BillingHistoryResponse>
    {
        public Guid UserId { get; }

        public GetBillingHistoryQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetBillingHistoryQueryHandler : IRequestHandler<GetBillingHistoryQuery, BillingHistoryResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<GetBillingHistoryQueryHandler> _logger;

        public GetBillingHistoryQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<GetBillingHistoryQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<BillingHistoryResponse> Handle(GetBillingHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var payments = await _subscriptionService.GetUserPaymentHistoryAsync(request.UserId);

                if (payments == null || payments.Count == 0)
                {
                    return new BillingHistoryResponse
                    {
                        Success = true,
                        Message = "No payment history found for this user.",
                        Data = new List<PaymentDto>()
                    };
                }

                return new BillingHistoryResponse
                {
                    Success = true,
                    Message = "Payment history retrieved successfully.",
                    Data = payments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history for user: {UserId}", request.UserId);
                return new BillingHistoryResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the payment history: " + ex.Message
                };
            }
        }
    }
}