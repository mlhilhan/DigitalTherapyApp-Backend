using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Queries
{
    public class CheckFeatureAccessQuery : IRequest<FeatureAccessResponse>
    {
        public Guid UserId { get; }
        public string FeatureName { get; }

        public CheckFeatureAccessQuery(Guid userId, string featureName)
        {
            UserId = userId;
            FeatureName = featureName;
        }
    }

    public class CheckFeatureAccessQueryHandler : IRequestHandler<CheckFeatureAccessQuery, FeatureAccessResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<CheckFeatureAccessQueryHandler> _logger;

        public CheckFeatureAccessQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<CheckFeatureAccessQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<FeatureAccessResponse> Handle(CheckFeatureAccessQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var hasAccess = await _subscriptionService.CheckUserFeatureAccessAsync(request.UserId, request.FeatureName);

                return new FeatureAccessResponse
                {
                    Success = true,
                    Message = "Feature access checked successfully.",
                    HasAccess = hasAccess
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking feature access for user: {UserId}, feature: {FeatureName}",
                    request.UserId, request.FeatureName);

                return new FeatureAccessResponse
                {
                    Success = false,
                    Message = "An error occurred while checking feature access: " + ex.Message,
                    HasAccess = false
                };
            }
        }
    }
}