using DigitalTherapyBackendApp.Api.Features.DailyTips.Responses;
using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Commands
{
    public class CreateDailyTipCommand : IRequest<CreateDailyTipResponse>
    {
        public string TipKey { get; set; }
        public int CategoryId { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsFeatured { get; set; }
        public Dictionary<string, TipTranslationPayload> Translations { get; set; }
    }

    public class TipTranslationPayload
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
    }

    public class CreateDailyTipCommandHandler : IRequestHandler<CreateDailyTipCommand, CreateDailyTipResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<CreateDailyTipCommandHandler> _logger;

        public CreateDailyTipCommandHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<CreateDailyTipCommandHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<CreateDailyTipResponse> Handle(
            CreateDailyTipCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.TipKey))
                {
                    return new CreateDailyTipResponse
                    {
                        Success = false,
                        Message = "TipKey is required."
                    };
                }

                if (request.Translations == null || request.Translations.Count == 0)
                {
                    return new CreateDailyTipResponse
                    {
                        Success = false,
                        Message = "At least one translation is required."
                    };
                }

                // Check if category exists
                var category = await _dailyTipRepository.GetCategoryByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return new CreateDailyTipResponse
                    {
                        Success = false,
                        Message = $"Category with ID {request.CategoryId} not found."
                    };
                }

                // Create entity
                var tip = new DailyTip
                {
                    CategoryId = request.CategoryId,
                    TipKey = request.TipKey,
                    Icon = request.Icon,
                    Color = request.Color,
                    IsFeatured = request.IsFeatured,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Translations = new List<DailyTipTranslation>()
                };

                // Add translations
                foreach (var translation in request.Translations)
                {
                    tip.Translations.Add(new DailyTipTranslation
                    {
                        LanguageCode = translation.Key,
                        Title = translation.Value.Title,
                        ShortDescription = translation.Value.ShortDescription,
                        Content = translation.Value.Content,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                var createdTip = await _dailyTipRepository.CreateTipAsync(tip);

                // Map to DTO for response
                string defaultLanguage = request.Translations.Keys.FirstOrDefault() ?? "tr";
                var defaultTranslation = request.Translations[defaultLanguage];

                return new CreateDailyTipResponse
                {
                    Success = true,
                    Message = "Daily tip created successfully.",
                    Data = new DailyTipDto
                    {
                        Id = createdTip.Id,
                        TipKey = createdTip.TipKey,
                        Title = defaultTranslation.Title,
                        ShortDescription = defaultTranslation.ShortDescription,
                        Content = defaultTranslation.Content,
                        Icon = createdTip.Icon,
                        Color = createdTip.Color,
                        IsFeatured = createdTip.IsFeatured,
                        IsBookmarked = false,
                        Category = new DailyTipCategoryDto
                        {
                            Id = category.Id,
                            CategoryKey = category.CategoryKey,
                            Name = category.Translations.FirstOrDefault()?.Name ?? "Unknown Category",
                            Icon = category.Icon,
                            Color = category.Color
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating daily tip");
                return new CreateDailyTipResponse
                {
                    Success = false,
                    Message = "An error occurred while creating the daily tip."
                };
            }
        }
    }
}