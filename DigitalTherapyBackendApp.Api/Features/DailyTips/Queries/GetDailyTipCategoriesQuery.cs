using DigitalTherapyBackendApp.Api.Features.DailyTips.Responses;
using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Queries
{
    public class GetDailyTipCategoriesQuery : IRequest<GetDailyTipCategoriesResponse>
    {
        public string LanguageCode { get; }

        public GetDailyTipCategoriesQuery(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }

    public class GetDailyTipCategoriesQueryHandler : IRequestHandler<GetDailyTipCategoriesQuery, GetDailyTipCategoriesResponse>
    {
        private readonly IDailyTipRepository _dailyTipRepository;
        private readonly ILogger<GetDailyTipCategoriesQueryHandler> _logger;

        public GetDailyTipCategoriesQueryHandler(
            IDailyTipRepository dailyTipRepository,
            ILogger<GetDailyTipCategoriesQueryHandler> logger)
        {
            _dailyTipRepository = dailyTipRepository;
            _logger = logger;
        }

        public async Task<GetDailyTipCategoriesResponse> Handle(
            GetDailyTipCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _dailyTipRepository.GetAllCategoriesAsync();

                var categoriesDto = categories.Select(c => new DailyTipCategoryDto
                {
                    Id = c.Id,
                    CategoryKey = c.CategoryKey,
                    Name = c.Translations
                        .FirstOrDefault(t => t.LanguageCode == request.LanguageCode)?.Name
                        ?? c.Translations.FirstOrDefault()?.Name
                        ?? "Unknown Category",
                    Icon = c.Icon,
                    Color = c.Color
                }).ToList();

                return new GetDailyTipCategoriesResponse
                {
                    Success = true,
                    Data = categoriesDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily tip categories");
                return new GetDailyTipCategoriesResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving daily tip categories."
                };
            }
        }
    }
}