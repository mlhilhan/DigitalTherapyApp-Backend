using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Queries
{
    public class GetAllUserSubscriptionsQuery : IRequest<AllUserSubscriptionsResponse>
    {
    }

    public class GetAllUserSubscriptionsQueryHandler : IRequestHandler<GetAllUserSubscriptionsQuery, AllUserSubscriptionsResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<GetAllUserSubscriptionsQueryHandler> _logger;

        public GetAllUserSubscriptionsQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<GetAllUserSubscriptionsQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<AllUserSubscriptionsResponse> Handle(GetAllUserSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subscriptions = await _subscriptionService.GetAllUserSubscriptionsAsync();

                if (subscriptions == null || subscriptions.Count == 0)
                {
                    return new AllUserSubscriptionsResponse
                    {
                        Success = true,
                        Message = "No user subscriptions found.",
                        Data = new List<UserSubscriptionDto>()
                    };
                }

                return new AllUserSubscriptionsResponse
                {
                    Success = true,
                    Message = "User subscriptions retrieved successfully.",
                    Data = subscriptions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user subscriptions");
                return new AllUserSubscriptionsResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving user subscriptions: " + ex.Message
                };
            }
        }
    }
}