using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Queries
{
    public class GetSubscriptionPlanQuery : IRequest<SubscriptionPlanResponse>
    {
        public int Id { get; }
        public string CountryCode { get; }
        public string LanguageCode { get; }

        public GetSubscriptionPlanQuery(int id, string countryCode, string languageCode)
        {
            Id = id;
            CountryCode = countryCode;
            LanguageCode = languageCode;
        }
    }

    public class GetSubscriptionPlanQueryHandler : IRequestHandler<GetSubscriptionPlanQuery, SubscriptionPlanResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<GetSubscriptionPlanQueryHandler> _logger;

        public GetSubscriptionPlanQueryHandler(
            ISubscriptionService subscriptionService,
            ILogger<GetSubscriptionPlanQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionPlanResponse> Handle(GetSubscriptionPlanQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = await _subscriptionService.GetSubscriptionDetailsAsync(
                    request.Id,
                    request.CountryCode,
                    request.LanguageCode);

                if (plan == null)
                {
                    return new SubscriptionPlanResponse
                    {
                        Success = false,
                        Message = $"Subscription plan with ID {request.Id} was not found."
                    };
                }

                return new SubscriptionPlanResponse
                {
                    Success = true,
                    Message = "Subscription plan retrieved successfully.",
                    Data = plan
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan with ID: {Id}", request.Id);
                return new SubscriptionPlanResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the subscription plan: " + ex.Message
                };
            }
        }
    }
}