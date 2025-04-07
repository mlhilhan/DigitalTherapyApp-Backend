using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Queries
{
    public class GetUserSubscriptionQuery : IRequest<SubscriptionResponse>
    {
        public Guid UserId { get; }

        public GetUserSubscriptionQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetUserSubscriptionQueryHandler : IRequestHandler<GetUserSubscriptionQuery, SubscriptionResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<GetUserSubscriptionQueryHandler> _logger;

        public GetUserSubscriptionQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<GetUserSubscriptionQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionResponse> Handle(GetUserSubscriptionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userSubscription = await _subscriptionService.GetUserActiveSubscriptionAsync(request.UserId);

                if (userSubscription == null)
                {
                    return new SubscriptionResponse
                    {
                        Success = false,
                        Message = "No active subscription found for this user."
                    };
                }

                return new SubscriptionResponse
                {
                    Success = true,
                    Message = "Active subscription retrieved successfully.",
                    Data = userSubscription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription for user: {UserId}", request.UserId);
                return new SubscriptionResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the active subscription: " + ex.Message
                };
            }
        }
    }
}