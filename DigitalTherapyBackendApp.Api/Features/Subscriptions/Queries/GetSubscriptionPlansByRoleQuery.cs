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
    public class GetSubscriptionPlansByRoleQuery : IRequest<SubscriptionPlansResponse>
    {
        public string RoleId { get; }
        public string CountryCode { get; }
        public string LanguageCode { get; }

        public GetSubscriptionPlansByRoleQuery(string roleId, string countryCode, string languageCode)
        {
            RoleId = roleId;
            CountryCode = countryCode;
            LanguageCode = languageCode;
        }
    }

    public class GetSubscriptionPlansByRoleQueryHandler : IRequestHandler<GetSubscriptionPlansByRoleQuery, SubscriptionPlansResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<GetSubscriptionPlansByRoleQueryHandler> _logger;

        public GetSubscriptionPlansByRoleQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<GetSubscriptionPlansByRoleQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionPlansResponse> Handle(GetSubscriptionPlansByRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var plans = await _subscriptionService.GetSubscriptionsWithDetailsByRoleAsync(
                    request.RoleId,
                    request.CountryCode,
                    request.LanguageCode);

                if (plans == null || plans.Count == 0)
                {
                    return new SubscriptionPlansResponse
                    {
                        Success = true,
                        Message = $"No subscription plans found for role {request.RoleId}.",
                        Data = new List<SubscriptionDetailsDto>()
                    };
                }

                return new SubscriptionPlansResponse
                {
                    Success = true,
                    Message = "Subscription plans retrieved successfully.",
                    Data = plans
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans for role {RoleId}", request.RoleId);
                return new SubscriptionPlansResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving subscription plans: " + ex.Message
                };
            }
        }
    }
}