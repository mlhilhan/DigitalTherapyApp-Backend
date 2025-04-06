using DigitalTherapyBackendApp.Api.Features.DailyTips.Responses;
using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Commands
{
    public class UpdateDailyTipCommand : IRequest<UpdateDailyTipResponse>
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsFeatured { get; set; }
        public Dictionary<string, TipTranslationPayload> Translations { get; set; }
    }

    public class UpdateDailyTipCommandHandler : IRequestHandler<UpdateDailyTipCommand, UpdateDailyTipResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<UpdateDailyTipCommandHandler> _logger;

        public UpdateDailyTipCommandHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<UpdateDailyTipCommandHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<UpdateDailyTipResponse> Handle(
            UpdateDailyTipCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.Translations == null || request.Translations.Count == 0)
                {
                    return new UpdateDailyTipResponse
                    {
                        Success = false,
                        Message = "At least one translation is required."
                    };
                }

                var tip = await _dailyTipRepository.GetTipByIdAsync(request.Id);
                if (tip == null)
                {
                    return new UpdateDailyTipResponse
                    {
                        Success = false,
                        Message = $"Tip with ID {request.Id} not found."
                    };
                }

                var category = await _dailyTipRepository.GetCategoryByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return new UpdateDailyTipResponse
                    {
                        Success = false,
                        Message = $"Category with ID {request.CategoryId} not found."
                    };
                }

                tip.CategoryId = request.CategoryId;
                tip.Icon = request.Icon;
                tip.Color = request.Color;
                tip.IsFeatured = request.IsFeatured;
                tip.UpdatedAt = DateTime.UtcNow;

                foreach (var translationPair in request.Translations)
                {
                    string languageCode = translationPair.Key;
                    TipTranslationPayload translationData = translationPair.Value;

                    var existingTranslation = tip.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
                    if (existingTranslation != null)
                    {
                        existingTranslation.Title = translationData.Title;
                        existingTranslation.ShortDescription = translationData.ShortDescription;
                        existingTranslation.Content = translationData.Content;
                        existingTranslation.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        tip.Translations.Add(new DailyTipTranslation
                        {
                            TipId = tip.Id,
                            LanguageCode = languageCode,
                            Title = translationData.Title,
                            ShortDescription = translationData.ShortDescription,
                            Content = translationData.Content,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                bool success = await _dailyTipRepository.UpdateTipAsync(tip);
                if (!success)
                {
                    return new UpdateDailyTipResponse
                    {
                        Success = false,
                        Message = "Failed to update the tip."
                    };
                }

                string defaultLanguage = request.Translations.Keys.FirstOrDefault() ?? "en";
                var defaultTranslation = request.Translations[defaultLanguage];

                return new UpdateDailyTipResponse
                {
                    Success = true,
                    Message = "Daily tip updated successfully.",
                    Data = new DailyTipDto
                    {
                        Id = tip.Id,
                        TipKey = tip.TipKey,
                        Title = defaultTranslation.Title,
                        ShortDescription = defaultTranslation.ShortDescription,
                        Content = defaultTranslation.Content,
                        Icon = tip.Icon,
                        Color = tip.Color,
                        IsFeatured = tip.IsFeatured,
                        IsBookmarked = tip.IsBookmarked,
                        Category = new DailyTipCategoryDto
                        {
                            Id = category.Id,
                            CategoryKey = category.CategoryKey,
                            Name = category.Translations.FirstOrDefault(t => t.LanguageCode == defaultLanguage)?.Name
                                ?? category.Translations.FirstOrDefault()?.Name
                                ?? "Unknown Category",
                            Icon = category.Icon,
                            Color = category.Color
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating daily tip with ID {TipId}", request.Id);
                return new UpdateDailyTipResponse
                {
                    Success = false,
                    Message = "An error occurred while updating the daily tip."
                };
            }
        }
    }
}