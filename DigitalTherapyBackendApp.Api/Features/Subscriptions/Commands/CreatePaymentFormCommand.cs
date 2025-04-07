using DigitalTherapyBackendApp.Api.Features.Subscriptions.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.Subscriptions.Commands
{
    public class CreatePaymentFormCommand : IRequest<PaymentFormResponse>
    {
        public Guid UserId { get; set; }
        public int SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string ReturnUrl { get; set; }
    }

    public class CreatePaymentFormCommandHandler : IRequestHandler<CreatePaymentFormCommand, PaymentFormResponse>
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<CreatePaymentFormCommandHandler> _logger;

        public CreatePaymentFormCommandHandler(
            IPaymentGatewayService paymentGatewayService,
            ISubscriptionService subscriptionService,
            ILogger<CreatePaymentFormCommandHandler> logger)
        {
            _paymentGatewayService = paymentGatewayService;
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<PaymentFormResponse> Handle(CreatePaymentFormCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(request.SubscriptionId);

                if (subscription == null)
                {
                    return new PaymentFormResponse
                    {
                        Success = false,
                        Message = "The requested subscription plan was not found."
                    };
                }

                var paymentForm = await _paymentGatewayService.CreatePaymentFormAsync(
                    request.UserId,
                    request.SubscriptionId,
                    request.Amount,
                    request.Currency,
                    request.ReturnUrl);

                if (!paymentForm.Success)
                {
                    return new PaymentFormResponse
                    {
                        Success = false,
                        Message = "Failed to create payment form: " + paymentForm.ErrorMessage
                    };
                }

                return new PaymentFormResponse
                {
                    Success = true,
                    Message = "Payment form was created successfully.",
                    Data = paymentForm
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment form for user: {UserId}, subscription: {SubscriptionId}",
                    request.UserId, request.SubscriptionId);

                return new PaymentFormResponse
                {
                    Success = false,
                    Message = "An error occurred while creating the payment form: " + ex.Message
                };
            }
        }
    }
}