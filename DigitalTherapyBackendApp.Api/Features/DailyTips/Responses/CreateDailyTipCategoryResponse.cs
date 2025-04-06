using DigitalTherapyBackendApp.Application.Dtos.DailyTips;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Responses
{
    public class CreateDailyTipCategoryResponse : BaseResponse
    {
        public DailyTipCategoryDto Data { get; set; }
    }
}