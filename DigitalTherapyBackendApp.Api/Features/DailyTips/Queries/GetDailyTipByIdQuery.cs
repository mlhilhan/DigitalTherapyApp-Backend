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
    public class GetDailyTipByIdQuery : IRequest<GetDailyTipResponse>
    {
        public int Id { get; }
        public string LanguageCode { get; }

        public GetDailyTipByIdQuery(int id, string languageCode)
        {
            Id = id;
            LanguageCode = languageCode;
        }
    }

    public class GetDailyTipByIdQueryHandler : IRequestHandler<GetDailyTipByIdQuery, GetDailyTipResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<GetDailyTipByIdQueryHandler> _logger;

        public GetDailyTipByIdQueryHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<GetDailyTipByIdQueryHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<GetDailyTipResponse> Handle(
            GetDailyTipByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var tip = await _dailyTipRepository.GetTipByIdAsync(request.Id);

                if (tip == null)
                {
                    return new GetDailyTipResponse
                    {
                        Success = false,
                        Message = $"Daily tip with ID {request.Id} not found."
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
                _logger.LogError(ex, "Error retrieving daily tip with ID {TipId}", request.Id);
                return new GetDailyTipResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the daily tip."
                };
            }
        }
    }
}