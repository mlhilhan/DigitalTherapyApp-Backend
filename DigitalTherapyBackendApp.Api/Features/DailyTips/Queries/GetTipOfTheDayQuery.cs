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
    public class GetTipOfTheDayQuery : IRequest<GetDailyTipResponse>
    {
        public string LanguageCode { get; }

        public GetTipOfTheDayQuery(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }

    public class GetTipOfTheDayQueryHandler : IRequestHandler<GetTipOfTheDayQuery, GetDailyTipResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<GetTipOfTheDayQueryHandler> _logger;

        public GetTipOfTheDayQueryHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<GetTipOfTheDayQueryHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<GetDailyTipResponse> Handle(
            GetTipOfTheDayQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tip = await _dailyTipRepository.GetTipOfTheDayAsync();

                if (tip == null)
                {
                    return new GetDailyTipResponse
                    {
                        Success = false,
                        Message = "No tip of the day available."
                    };
                }

                var translation = tip.Translations
                    .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)
                    ?? tip.Translations.FirstOrDefault();

                var categoryTranslation = tip.Category?.Translations
                    .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)
                    ?? tip.Category?.Translations.FirstOrDefault();

                var tipDto = new DailyTipDto
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

                return new GetDailyTipResponse
                {
                    Success = true,
                    Data = tipDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tip of the day");
                return new GetDailyTipResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the tip of the day."
                };
            }
        }
    }
}