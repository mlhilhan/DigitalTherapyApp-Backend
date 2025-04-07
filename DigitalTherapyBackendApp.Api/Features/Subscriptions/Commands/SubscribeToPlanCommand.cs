using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities.Subscriptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Commands
{
    public class SubscribeToPlanCommand : IRequest<SubscriptionResponse>
    {
        public Guid UserId { get; set; }
        public int SubscriptionId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class SubscribeToPlanCommandHandler : IRequestHandler<SubscribeToPlanCommand, SubscriptionResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscribeToPlanCommandHandler> _logger;

        public SubscribeToPlanCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<SubscribeToPlanCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionResponse> Handle(SubscribeToPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userSubscription = await _subscriptionService.SubscribeUserAsync(
                    request.UserId,
                    request.SubscriptionId,
                    request.PaymentMethod,
                    request.Amount,
                    request.TransactionId);

                if (userSubscription == null)
                {
                    return new SubscriptionResponse
                    {
                        Success = false,
                        Message = "Subscription process failed."
                    };
                }

                return new SubscriptionResponse
                {
                    Success = true,
                    Message = "Subscription was completed successfully.",
                    Data = userSubscription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during subscription process for user: {UserId}", request.UserId);
                return new SubscriptionResponse
                {
                    Success = false,
                    Message = "An error occurred during the subscription process: " + ex.Message
                };
            }
        }
    }
}