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
    public class UpdateDailyTipCategoryCommand : IRequest<UpdateDailyTipCategoryResponse>
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public Dictionary<string, string> Translations { get; set; }
    }

    public class UpdateDailyTipCategoryCommandHandler : IRequestHandler<UpdateDailyTipCategoryCommand, UpdateDailyTipCategoryResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<UpdateDailyTipCategoryCommandHandler> _logger;

        public UpdateDailyTipCategoryCommandHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<UpdateDailyTipCategoryCommandHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<UpdateDailyTipCategoryResponse> Handle(
            UpdateDailyTipCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.Translations == null || request.Translations.Count == 0)
                {
                    return new UpdateDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = "At least one translation is required."
                    };
                }

                var category = await _dailyTipRepository.GetCategoryByIdAsync(request.Id);
                if (category == null)
                {
                    return new UpdateDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = $"Category with ID {request.Id} not found."
                    };
                }

                category.Icon = request.Icon;
                category.Color = request.Color;
                category.UpdatedAt = DateTime.UtcNow;

                foreach (var translationPair in request.Translations)
                {
                    string languageCode = translationPair.Key;
                    string name = translationPair.Value;

                    var existingTranslation = category.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
                    if (existingTranslation != null)
                    {
                        existingTranslation.Name = name;
                        existingTranslation.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        category.Translations.Add(new DailyTipCategoryTranslation
                        {
                            CategoryId = category.Id,
                            LanguageCode = languageCode,
                            Name = name,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                bool success = await _dailyTipRepository.UpdateCategoryAsync(category);
                if (!success)
                {
                    return new UpdateDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = "Failed to update the category."
                    };
                }

                string defaultLanguage = request.Translations.Keys.FirstOrDefault() ?? "en";
                string defaultName = request.Translations.ContainsKey(defaultLanguage)
                    ? request.Translations[defaultLanguage]
                    : request.Translations.Values.FirstOrDefault();

                return new UpdateDailyTipCategoryResponse
                {
                    Success = true,
                    Message = "Category updated successfully.",
                    Data = new DailyTipCategoryDto
                    {
                        Id = category.Id,
                        CategoryKey = category.CategoryKey,
                        Name = defaultName ?? "Unknown Category",
                        Icon = category.Icon,
                        Color = category.Color
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", request.Id);
                return new UpdateDailyTipCategoryResponse
                {
                    Success = false,
                    Message = "An error occurred while updating the category."
                };
            }
        }
    }
}