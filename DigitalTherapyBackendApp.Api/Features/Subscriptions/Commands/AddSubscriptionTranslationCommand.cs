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
    public class AddSubscriptionTranslationCommand : IRequest<SubscriptionTranslationResponse>
    {
        public int SubscriptionId { get; set; }
        public string LanguageCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AddSubscriptionTranslationCommandHandler : IRequestHandler<AddSubscriptionTranslationCommand, SubscriptionTranslationResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<AddSubscriptionTranslationCommandHandler> _logger;

        public AddSubscriptionTranslationCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<AddSubscriptionTranslationCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionTranslationResponse> Handle(AddSubscriptionTranslationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(request.SubscriptionId);
                if (subscription == null)
                {
                    return new SubscriptionTranslationResponse
                    {
                        Success = false,
                        Message = $"Subscription with ID {request.SubscriptionId} was not found."
                    };
                }

                var translations = await _subscriptionService.GetSubscriptionTranslationsAsync(request.SubscriptionId);
                if (translations.Any(t => t.LanguageCode == request.LanguageCode))
                {
                    return new SubscriptionTranslationResponse
                    {
                        Success = false,
                        Message = $"A translation for language code '{request.LanguageCode}' already exists for this subscription."
                    };
                }

                var translationDto = new SubscriptionTranslationDto
                {
                    SubscriptionId = request.SubscriptionId,
                    LanguageCode = request.LanguageCode,
                    Name = request.Name,
                    Description = request.Description
                };

                var result = await _subscriptionService.AddSubscriptionTranslationAsync(translationDto);

                if (!result)
                {
                    return new SubscriptionTranslationResponse
                    {
                        Success = false,
                        Message = "Failed to add the subscription translation."
                    };
                }

                return new SubscriptionTranslationResponse
                {
                    Success = true,
                    Message = "Subscription translation was added successfully.",
                    Data = translationDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding subscription translation for subscription: {SubscriptionId}, language: {LanguageCode}",
                    request.SubscriptionId, request.LanguageCode);

                return new SubscriptionTranslationResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the subscription translation: " + ex.Message
                };
            }
        }
    }
}