using DigitalTherapyBackendApp.Application.Dtos.DailyTips;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Responses
{
    public class UpdateDailyTipResponse : BaseResponse
    {
        public DailyTipDto Data { get; set; }
    }
}