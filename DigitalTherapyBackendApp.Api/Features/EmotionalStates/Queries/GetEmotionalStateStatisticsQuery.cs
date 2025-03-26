using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries
{
    public class GetEmotionalStateStatisticsQuery : IRequest<GetEmotionalStateStatisticsResponse>
    {
        public Guid UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public GetEmotionalStateStatisticsQuery(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            UserId = userId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    public class GetEmotionalStateStatisticsQueryHandler : IRequestHandler<GetEmotionalStateStatisticsQuery, GetEmotionalStateStatisticsResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public GetEmotionalStateStatisticsQueryHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<GetEmotionalStateStatisticsResponse> Handle(GetEmotionalStateStatisticsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var statistics = await _emotionalStateService.GetStatisticsAsync(query.UserId, query.StartDate, query.EndDate);

                return new GetEmotionalStateStatisticsResponse
                {
                    Success = true,
                    Message = "Mood statistics retrieved successfully.",
                    Data = statistics
                };
            }
            catch (Exception ex)
            {
                return new GetEmotionalStateStatisticsResponse
                {
                    Success = false,
                    Message = $"Failed to retrieve mood statistics: {ex.Message}",
                    ErrorCode = "GET_EMOTIONALSTATE_STATISTICS_ERROR",
                    Data = null
                };
            }
        }
    }
}