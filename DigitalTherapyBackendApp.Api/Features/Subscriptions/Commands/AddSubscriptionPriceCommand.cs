using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Commands
{
    public class AddSubscriptionPriceCommand : IRequest<SubscriptionPriceResponse>
    {
        public int SubscriptionId { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Price { get; set; }
    }

    public class AddSubscriptionPriceCommandHandler : IRequestHandler<AddSubscriptionPriceCommand, SubscriptionPriceResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<AddSubscriptionPriceCommandHandler> _logger;

        public AddSubscriptionPriceCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<AddSubscriptionPriceCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionPriceResponse> Handle(AddSubscriptionPriceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(request.SubscriptionId);
                if (subscription == null)
                {
                    return new SubscriptionPriceResponse
                    {
                        Success = false,
                        Message = $"Subscription with ID {request.SubscriptionId} was not found."
                    };
                }

                var prices = await _subscriptionService.GetSubscriptionPricesAsync(request.SubscriptionId);
                if (prices.Any(p => p.CountryCode == request.CountryCode))
                {
                    return new SubscriptionPriceResponse
                    {
                        Success = false,
                        Message = $"A price for country code '{request.CountryCode}' already exists for this subscription."
                    };
                }

                var priceDto = new SubscriptionPriceDto
                {
                    SubscriptionId = request.SubscriptionId,
                    CountryCode = request.CountryCode,
                    CurrencyCode = request.CurrencyCode,
                    Price = request.Price
                };

                var result = await _subscriptionService.AddSubscriptionPriceAsync(priceDto);

                if (!result)
                {
                    return new SubscriptionPriceResponse
                    {
                        Success = false,
                        Message = "Failed to add the subscription price."
                    };
                }

                return new SubscriptionPriceResponse
                {
                    Success = true,
                    Message = "Subscription price was added successfully.",
                    Data = priceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding subscription price for subscription: {SubscriptionId}, country: {CountryCode}",
                    request.SubscriptionId, request.CountryCode);

                return new SubscriptionPriceResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the subscription price: " + ex.Message
                };
            }
        }
    }
}