using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Commands
{
    public class ProcessPaymentWebhookCommand : IRequest<WebhookResponse>
    {
        public string WebhookData { get; set; }
    }

    public class ProcessPaymentWebhookCommandHandler : IRequestHandler<ProcessPaymentWebhookCommand, WebhookResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<ProcessPaymentWebhookCommandHandler> _logger;

        public ProcessPaymentWebhookCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<ProcessPaymentWebhookCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<WebhookResponse> Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _subscriptionService.ProcessPaymentWebhookAsync(request.WebhookData);

                if (!result)
                {
                    return new WebhookResponse
                    {
                        Success = false,
                        Message = "Failed to process payment webhook."
                    };
                }

                return new WebhookResponse
                {
                    Success = true,
                    Message = "Payment webhook was processed successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment webhook");
                return new WebhookResponse
                {
                    Success = false,
                    Message = "An error occurred while processing the payment webhook: " + ex.Message
                };
            }
        }
    }
}