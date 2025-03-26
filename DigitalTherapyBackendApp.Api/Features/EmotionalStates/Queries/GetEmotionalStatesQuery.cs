using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries
{
    public class GetEmotionalStatesQuery : IRequest<GetEmotionalStatesResponse>
    {
        public Guid UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public GetEmotionalStatesQuery(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            UserId = userId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    public class GetEmotionalStatesQueryHandler : IRequestHandler<GetEmotionalStatesQuery, GetEmotionalStatesResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public GetEmotionalStatesQueryHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<GetEmotionalStatesResponse> Handle(GetEmotionalStatesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<EmotionalStateDto> emotionalStates;

                if (query.StartDate.HasValue && query.EndDate.HasValue)
                {
                    emotionalStates = await _emotionalStateService.GetByDateRangeAsync(query.UserId, query.StartDate.Value, query.EndDate.Value);
                }
                else
                {
                    emotionalStates = await _emotionalStateService.GetAllByUserIdAsync(query.UserId);
                }

                return new GetEmotionalStatesResponse
                {
                    Success = true,
                    Message = "Mood records retrieved successfully.",
                    Data = emotionalStates
                };
            }
            catch (Exception ex)
            {
                return new GetEmotionalStatesResponse
                {
                    Success = false,
                    Message = $"Failed to retrieve mood records: {ex.Message}",
                    ErrorCode = "GET_EMOTIONALSTATES_ERROR",
                    Data = null
                };
            }
        }
    }
}