using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Queries
{
    public class GetFeatureLimitQuery : IRequest<FeatureLimitResponse>
    {
        public Guid UserId { get; }
        public string FeatureName { get; }

        public GetFeatureLimitQuery(Guid userId, string featureName)
        {
            UserId = userId;
            FeatureName = featureName;
        }
    }

    public class GetFeatureLimitQueryHandler : IRequestHandler<GetFeatureLimitQuery, FeatureLimitResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<GetFeatureLimitQueryHandler> _logger;

        public GetFeatureLimitQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<GetFeatureLimitQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<FeatureLimitResponse> Handle(GetFeatureLimitQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var limit = await _subscriptionService.GetUserFeatureLimitAsync(request.UserId, request.FeatureName);

                return new FeatureLimitResponse
                {
                    Success = true,
                    Message = "Feature limit retrieved successfully.",
                    Limit = limit
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature limit for user: {UserId}, feature: {FeatureName}",
                    request.UserId, request.FeatureName);

                return new FeatureLimitResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the feature limit: " + ex.Message,
                    Limit = 0
                };
            }
        }
    }
}