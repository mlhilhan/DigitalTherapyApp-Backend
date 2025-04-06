using DigitalTherapyBackendApp.Api.Features.DailyTips.Responses;
using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Queries
{
    public class GetDailyTipsByCategoryQuery : IRequest<GetDailyTipsResponse>
    {
        public string CategoryKey { get; }
        public string LanguageCode { get; }

        public GetDailyTipsByCategoryQuery(string categoryKey, string languageCode)
        {
            CategoryKey = categoryKey;
            LanguageCode = languageCode;
        }
    }

    public class GetDailyTipsByCategoryQueryHandler : IRequestHandler<GetDailyTipsByCategoryQuery, GetDailyTipsResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<GetDailyTipsByCategoryQueryHandler> _logger;

        public GetDailyTipsByCategoryQueryHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<GetDailyTipsByCategoryQueryHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<GetDailyTipsResponse> Handle(
            GetDailyTipsByCategoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tips = await _dailyTipRepository.GetTipsByCategoryAsync(request.CategoryKey);

                if (tips == null || !tips.Any())
                {
                    return new GetDailyTipsResponse
                    {
                        Success = true,
                        Data = new System.Collections.Generic.List<DailyTipDto>(),
                        Message = $"No tips found for category '{request.CategoryKey}'."
                    };
                }

                var tipsDto = tips.Select(t =>
                {
                    var translation = t.Translations
                        .FirstOrDefault(tr => tr.LanguageCode == request.LanguageCode)
                        ?? t.Translations.FirstOrDefault();

                    var categoryTranslation = t.Category?.Translations
                        .FirstOrDefault(tr => tr.LanguageCode == request.LanguageCode)
                        ?? t.Category?.Translations.FirstOrDefault();

                    return new DailyTipDto
                    {
                        Id = t.Id,
                        TipKey = t.TipKey,
                        Title = translation?.Title ?? "Unknown Title",
                        ShortDescription = translation?.ShortDescription ?? "",
                        Content = translation?.Content ?? "",
                        Icon = t.Icon,
                        Color = t.Color,
                        IsFeatured = t.IsFeatured,
                        IsBookmarked = t.IsBookmarked,
                        Category = t.Category != null ? new DailyTipCategoryDto
                        {
                            Id = t.Category.Id,
                            CategoryKey = t.Category.CategoryKey,
                            Name = categoryTranslation?.Name ?? "Unknown Category",
                            Icon = t.Category.Icon,
                            Color = t.Category.Color
                        } : null
                    };
                }).ToList();

                return new GetDailyTipsResponse
                {
                    Success = true,
                    Data = tipsDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily tips for category {CategoryKey}", request.CategoryKey);
                return new GetDailyTipsResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving daily tips."
                };
            }
        }
    }
}