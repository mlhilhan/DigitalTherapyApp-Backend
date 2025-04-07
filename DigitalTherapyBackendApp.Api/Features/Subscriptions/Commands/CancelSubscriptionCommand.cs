using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Commands
{
    public class CancelSubscriptionCommand : IRequest<SubscriptionResponse>
    {
        public Guid UserId { get; }

        public CancelSubscriptionCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, SubscriptionResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<CancelSubscriptionCommandHandler> _logger;

        public CancelSubscriptionCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<CancelSubscriptionCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionResponse> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userSubscription = await _subscriptionService.GetUserActiveSubscriptionAsync(request.UserId);

                if (userSubscription == null)
                {
                    return new SubscriptionResponse
                    {
                        Success = false,
                        Message = "No active subscription found."
                    };
                }

                var result = await _subscriptionService.CancelSubscriptionAsync(userSubscription.Id);

                if (!result)
                {
                    return new SubscriptionResponse
                    {
                        Success = false,
                        Message = "Failed to cancel subscription."
                    };
                }

                return new SubscriptionResponse
                {
                    Success = true,
                    Message = "Subscription was canceled successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling subscription for user: {UserId}", request.UserId);
                return new SubscriptionResponse
                {
                    Success = false,
                    Message = "An error occurred while canceling the subscription: " + ex.Message
                };
            }
        }
    }
}