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
    public class UpdateSubscriptionPlanCommand : IRequest<SubscriptionPlanResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string BaseCurrency { get; set; }
        public string CountryCode { get; set; } = "US";
        public string LanguageCode { get; set; } = "en";
        public int DurationInDays { get; set; }
        public bool IsActive { get; set; }
        public int MoodEntryLimit { get; set; }
        public int AIChatSessionsPerWeek { get; set; }
        public int MessageLimitPerChat { get; set; }
        public bool HasPsychologistSupport { get; set; }
        public int PsychologistSessionsPerMonth { get; set; }
        public bool HasEmergencySupport { get; set; }
        public bool HasAdvancedReports { get; set; }
        public bool HasAllMeditationContent { get; set; }
    }

    public class UpdateSubscriptionPlanCommandHandler : IRequestHandler<UpdateSubscriptionPlanCommand, SubscriptionPlanResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<UpdateSubscriptionPlanCommandHandler> _logger;

        public UpdateSubscriptionPlanCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<UpdateSubscriptionPlanCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionPlanResponse> Handle(UpdateSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingPlan = await _subscriptionService.GetSubscriptionByIdAsync(request.Id);
                if (existingPlan == null)
                {
                    return new SubscriptionPlanResponse
                    {
                        Success = false,
                        Message = $"Subscription plan with ID {request.Id} was not found."
                    };
                }

                var subscriptionDto = new SubscriptionDto
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    BasePrice = request.BasePrice,
                    BaseCurrency = request.BaseCurrency,
                    PlanId = existingPlan.PlanId,
                    DurationInDays = request.DurationInDays,
                    IsActive = request.IsActive,
                    UpdatedAt = DateTime.UtcNow,
                    MoodEntryLimit = request.MoodEntryLimit,
                    AIChatSessionsPerWeek = request.AIChatSessionsPerWeek,
                    MessageLimitPerChat = request.MessageLimitPerChat,
                    HasPsychologistSupport = request.HasPsychologistSupport,
                    PsychologistSessionsPerMonth = request.PsychologistSessionsPerMonth,
                    HasEmergencySupport = request.HasEmergencySupport,
                    HasAdvancedReports = request.HasAdvancedReports,
                    HasAllMeditationContent = request.HasAllMeditationContent
                };

                var result = await _subscriptionService.UpdateSubscriptionPlanAsync(subscriptionDto);

                if (result == null)
                {
                    return new SubscriptionPlanResponse
                    {
                        Success = false,
                        Message = "Failed to update the subscription plan."
                    };
                }

                return new SubscriptionPlanResponse
                {
                    Success = true,
                    Message = "Subscription plan was updated successfully.",
                    Data = await _subscriptionService.GetSubscriptionDetailsAsync(request.Id, request.CountryCode, request.LanguageCode)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", request.Id);
                return new SubscriptionPlanResponse
                {
                    Success = false,
                    Message = "An error occurred while updating the subscription plan: " + ex.Message
                };
            }
        }
    }
}