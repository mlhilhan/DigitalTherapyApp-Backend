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
    public class CreateDailyTipCategoryCommand : IRequest<CreateDailyTipCategoryResponse>
    {
        public string CategoryKey { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public Dictionary<string, string> Translations { get; set; }
    }

    public class CreateDailyTipCategoryCommandHandler : IRequestHandler<CreateDailyTipCategoryCommand, CreateDailyTipCategoryResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<CreateDailyTipCategoryCommandHandler> _logger;

        public CreateDailyTipCategoryCommandHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<CreateDailyTipCategoryCommandHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<CreateDailyTipCategoryResponse> Handle(
            CreateDailyTipCategoryCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CategoryKey))
                {
                    return new CreateDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = "CategoryKey is required."
                    };
                }

                if (request.Translations == null || request.Translations.Count == 0)
                {
                    return new CreateDailyTipCategoryResponse
                    {
                        Success = false,
                        Message = "At least one translation is required."
                    };
                }

                var category = new DailyTipCategory
                {
                    CategoryKey = request.CategoryKey,
                    Icon = request.Icon,
                    Color = request.Color,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Translations = new List<DailyTipCategoryTranslation>()
                };

                foreach (var translation in request.Translations)
                {
                    category.Translations.Add(new DailyTipCategoryTranslation
                    {
                        LanguageCode = translation.Key,
                        Name = translation.Value,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                var createdCategory = await _dailyTipRepository.CreateCategoryAsync(category);

                string defaultLanguage = request.Translations.Keys.FirstOrDefault() ?? "en";
                string defaultName = request.Translations.ContainsKey(defaultLanguage)
                    ? request.Translations[defaultLanguage]
                    : request.Translations.Values.FirstOrDefault();

                return new CreateDailyTipCategoryResponse
                {
                    Success = true,
                    Message = "Daily tip category created successfully.",
                    Data = new DailyTipCategoryDto
                    {
                        Id = createdCategory.Id,
                        CategoryKey = createdCategory.CategoryKey,
                        Name = defaultName ?? "Category",
                        Icon = createdCategory.Icon,
                        Color = createdCategory.Color
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating daily tip category");
                return new CreateDailyTipCategoryResponse
                {
                    Success = false,
                    Message = "An error occurred while creating the daily tip category."
                };
            }
        }
    }
}