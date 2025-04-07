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
    public class GetSubscriptionPlansQuery : IRequest<SubscriptionPlansResponse>
    {
        public string CountryCode { get; }
        public string LanguageCode { get; }

        public GetSubscriptionPlansQuery(string countryCode, string languageCode)
        {
            CountryCode = countryCode;
            LanguageCode = languageCode;
        }
    }

    public class GetSubscriptionPlansQueryHandler : IRequestHandler<GetSubscriptionPlansQuery, SubscriptionPlansResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<GetSubscriptionPlansQueryHandler> _logger;

        public GetSubscriptionPlansQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<GetSubscriptionPlansQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionPlansResponse> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var plans = await _subscriptionService.GetSubscriptionsWithDetailsAsync(
                    request.CountryCode,
                    request.LanguageCode);

                if (plans == null || plans.Count == 0)
                {
                    return new SubscriptionPlansResponse
                    {
                        Success = true,
                        Message = "No subscription plans found.",
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
                _logger.LogError(ex, "Error retrieving subscription plans");
                return new SubscriptionPlansResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving subscription plans: " + ex.Message
                };
            }
        }
    }
}