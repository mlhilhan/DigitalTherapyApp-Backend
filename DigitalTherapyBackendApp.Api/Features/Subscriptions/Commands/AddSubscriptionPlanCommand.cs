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
    public class AddSubscriptionPlanCommand : IRequest<SubscriptionPlanResponse>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string BaseCurrency { get; set; } = "USD";
        public string PlanId { get; set; }
        public int DurationInDays { get; set; } = 30;
        public int MoodEntryLimit { get; set; }
        public int AIChatSessionsPerWeek { get; set; }
        public int MessageLimitPerChat { get; set; }
        public bool HasPsychologistSupport { get; set; }
        public int PsychologistSessionsPerMonth { get; set; }
        public bool HasEmergencySupport { get; set; }
        public bool HasAdvancedReports { get; set; }
        public bool HasAllMeditationContent { get; set; }
    }

    public class AddSubscriptionPlanCommandHandler : IRequestHandler<AddSubscriptionPlanCommand, SubscriptionPlanResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<AddSubscriptionPlanCommandHandler> _logger;

        public AddSubscriptionPlanCommandHandler(
            ISubscriptionService subscriptionService,
            ILogger<AddSubscriptionPlanCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<SubscriptionPlanResponse> Handle(AddSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingPlan = await _subscriptionService.GetSubscriptionByPlanIdAsync(request.PlanId);
                if (existingPlan != null)
                {
                    return new SubscriptionPlanResponse
                    {
                        Success = false,
                        Message = $"A subscription plan with ID '{request.PlanId}' already exists."
                    };
                }

                var subscriptionDto = new SubscriptionDto
                {
                    Name = request.Name,
                    Description = request.Description,
                    BasePrice = request.BasePrice,
                    BaseCurrency = request.BaseCurrency,
                    PlanId = request.PlanId,
                    DurationInDays = request.DurationInDays,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
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

                var result = await _subscriptionService.AddSubscriptionPlanAsync(subscriptionDto);

                if (result == null)
                {
                    return new SubscriptionPlanResponse
                    {
                        Success = false,
                        Message = "Failed to add the subscription plan."
                    };
                }

                return new SubscriptionPlanResponse
                {
                    Success = true,
                    Message = "Subscription plan was added successfully.",
                    Data = new SubscriptionDetailsDto
                    {
                        Id = result.Id,
                        PlanId = result.PlanId,
                        Name = result.Name,
                        Description = result.Description,
                        Price = result.BasePrice,
                        CurrencyCode = result.BaseCurrency,
                        DurationInDays = result.DurationInDays,
                        MoodEntryLimit = result.MoodEntryLimit,
                        AIChatSessionsPerWeek = result.AIChatSessionsPerWeek,
                        MessageLimitPerChat = result.MessageLimitPerChat,
                        HasPsychologistSupport = result.HasPsychologistSupport,
                        PsychologistSessionsPerMonth = result.PsychologistSessionsPerMonth,
                        HasEmergencySupport = result.HasEmergencySupport,
                        HasAdvancedReports = result.HasAdvancedReports,
                        HasAllMeditationContent = result.HasAllMeditationContent
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding subscription plan: {PlanId}", request.PlanId);
                return new SubscriptionPlanResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the subscription plan: " + ex.Message
                };
            }
        }
    }
}