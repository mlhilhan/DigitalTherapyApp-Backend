using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Commands
{
    public class ToggleAutoRenewCommand : IRequest<SubscriptionResponse>
    {
        public Guid UserId { get; set; }
        public bool AutoRenew { get; set; }
    }

    public class ToggleAutoRenewCommandHandler : IRequestHandler<ToggleAutoRenewCommand, SubscriptionResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<ToggleAutoRenewCommandHandler> _logger;

        public ToggleAutoRenewCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<ToggleAutoRenewCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionResponse> Handle(ToggleAutoRenewCommand request, CancellationToken cancellationToken)
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

                var result = await _subscriptionService.ToggleAutoRenewAsync(userSubscription.Id, request.AutoRenew);

                if (!result)
                {
                    return new SubscriptionResponse
                    {
                        Success = false,
                        Message = "Failed to update auto-renewal settings."
                    };
                }

                var statusMessage = request.AutoRenew
                    ? "Auto-renewal was enabled successfully."
                    : "Auto-renewal was disabled successfully.";

                return new SubscriptionResponse
                {
                    Success = true,
                    Message = statusMessage,
                    Data = await _subscriptionService.GetUserActiveSubscriptionAsync(request.UserId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling auto-renew for user: {UserId}", request.UserId);
                return new SubscriptionResponse
                {
                    Success = false,
                    Message = "An error occurred while updating auto-renewal settings: " + ex.Message
                };
            }
        }
    }
}