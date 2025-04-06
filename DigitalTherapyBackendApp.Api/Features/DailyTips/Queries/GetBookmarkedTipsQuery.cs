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
    public class GetBookmarkedTipsQuery : IRequest<GetDailyTipsResponse>
    {
        public string LanguageCode { get; }

        public GetBookmarkedTipsQuery(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }

    public class GetBookmarkedTipsQueryHandler : IRequestHandler<GetBookmarkedTipsQuery, GetDailyTipsResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<GetBookmarkedTipsQueryHandler> _logger;

        public GetBookmarkedTipsQueryHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<GetBookmarkedTipsQueryHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<GetDailyTipsResponse> Handle(
            GetBookmarkedTipsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tips = await _dailyTipRepository.GetBookmarkedTipsAsync();

                if (tips == null || !tips.Any())
                {
                    return new GetDailyTipsResponse
                    {
                        Success = true,
                        Data = new System.Collections.Generic.List<DailyTipDto>(),
                        Message = "No bookmarked tips found."
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
                _logger.LogError(ex, "Error retrieving bookmarked tips");
                return new GetDailyTipsResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving bookmarked tips."
                };
            }
        }
    }
}