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
    public class GetBookmarkedEmotionalStatesQuery : IRequest<GetEmotionalStatesResponse>
    {
        public Guid UserId { get; set; }

        public GetBookmarkedEmotionalStatesQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetBookmarkedEmotionalStatesQueryHandler : IRequestHandler<GetBookmarkedEmotionalStatesQuery, GetEmotionalStatesResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public GetBookmarkedEmotionalStatesQueryHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<GetEmotionalStatesResponse> Handle(GetBookmarkedEmotionalStatesQuery query, CancellationToken cancellationToken)
        {
            try
            {
                List<EmotionalStateDto> bookmarkedStates = await _emotionalStateService.GetBookmarkedAsync(query.UserId);

                return new GetEmotionalStatesResponse
                {
                    Success = true,
                    Message = "Bookmarked mood records retrieved successfully.",
                    Data = bookmarkedStates
                };
            }
            catch (Exception ex)
            {
                return new GetEmotionalStatesResponse
                {
                    Success = false,
                    Message = $"Failed to retrieve bookmarked mood records: {ex.Message}",
                    ErrorCode = "GET_BOOKMARKED_EMOTIONALSTATES_ERROR",
                    Data = null
                };
            }
        }
    }
}