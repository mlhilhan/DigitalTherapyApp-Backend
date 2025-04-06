using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Api.Features.DailyTips.Responses
{
    public class GetDailyTipsResponse : BaseResponse
    {
        public List<DailyTipDto> Data { get; set; }
    }
}