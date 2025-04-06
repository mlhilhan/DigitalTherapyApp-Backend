using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Responses
{
    public class GetDailyTipCategoriesResponse : BaseResponse
    {
        public List<DailyTipCategoryDto> Data { get; set; }
    }
}