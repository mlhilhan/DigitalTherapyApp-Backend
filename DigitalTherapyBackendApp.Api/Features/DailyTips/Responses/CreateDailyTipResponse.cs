using DigitalTherapyBackendApp.Application.Dtos.DailyTips;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Responses
{
    public class CreateDailyTipResponse : BaseResponse
    {
        public DailyTipDto Data { get; set; }
    }
}