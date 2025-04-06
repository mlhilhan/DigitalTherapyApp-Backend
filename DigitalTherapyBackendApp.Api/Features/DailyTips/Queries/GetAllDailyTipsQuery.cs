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
    public class GetAllDailyTipsQuery : IRequest<GetDailyTipsResponse>
    {
        public string LanguageCode { get; }

        public GetAllDailyTipsQuery(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }

    public class GetAllDailyTipsQueryHandler : IRequestHandler<GetAllDailyTipsQuery, GetDailyTipsResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<GetAllDailyTipsQueryHandler> _logger;

        public GetAllDailyTipsQueryHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<GetAllDailyTipsQueryHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<GetDailyTipsResponse> Handle(
            GetAllDailyTipsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tips = await _dailyTipRepository.GetAllTipsAsync();

                var tipsDto = tips.Select(t => MapTipToDto(t, request.LanguageCode)).ToList();

                return new GetDailyTipsResponse
                {
                    Success = true,
                    Data = tipsDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily tips");
                return new GetDailyTipsResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving daily tips."
                };
            }
        }

        private DailyTipDto MapTipToDto(DigitalTherapyBackendApp.Domain.Entities.DailyTip tip, string languageCode)
        {
            var translation = tip.Translations
                .FirstOrDefault(t => t.LanguageCode == languageCode)
                ?? tip.Translations.FirstOrDefault();

            var categoryTranslation = tip.Category?.Translations
                .FirstOrDefault(t => t.LanguageCode == languageCode)
                ?? tip.Category?.Translations.FirstOrDefault();

            return new DailyTipDto
            {
                Id = tip.Id,
                TipKey = tip.TipKey,
                Title = translation?.Title ?? "Unknown Title",
                ShortDescription = translation?.ShortDescription ?? "",
                Content = translation?.Content ?? "",
                Icon = tip.Icon,
                Color = tip.Color,
                IsFeatured = tip.IsFeatured,
                IsBookmarked = tip.IsBookmarked,
                Category = tip.Category != null ? new DailyTipCategoryDto
                {
                    Id = tip.Category.Id,
                    CategoryKey = tip.Category.CategoryKey,
                    Name = categoryTranslation?.Name ?? "Unknown Category",
                    Icon = tip.Category.Icon,
                    Color = tip.Category.Color
                } : null
            };
        }
    }
}