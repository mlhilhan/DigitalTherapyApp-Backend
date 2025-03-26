using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries
{
    public class GetEmotionalStateQuery : IRequest<GetEmotionalStateResponse>
    {
        public Guid Id { get; }
        public Guid UserId { get; }

        public GetEmotionalStateQuery(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
    }

    public class GetEmotionalStateQueryHandler : IRequestHandler<GetEmotionalStateQuery, GetEmotionalStateResponse>
    {
        private readonly IEmotionalStateService _emotionalStateService;

        public GetEmotionalStateQueryHandler(IEmotionalStateService emotionalStateService)
        {
            _emotionalStateService = emotionalStateService;
        }

        public async Task<GetEmotionalStateResponse> Handle(GetEmotionalStateQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var emotionalState = await _emotionalStateService.GetByIdAsync(query.Id, query.UserId);

                if (emotionalState == null)
                {
                    return new GetEmotionalStateResponse
                    {
                        Success = false,
                        Message = "The specified mood record was not found.",
                        ErrorCode = "EMOTIONALSTATE_NOT_FOUND",
                        Data = null
                    };
                }

                return new GetEmotionalStateResponse
                {
                    Success = true,
                    Message = "Mood record retrieved successfully.",
                    Data = emotionalState
                };
            }
            catch (Exception ex)
            {
                return new GetEmotionalStateResponse
                {
                    Success = false,
                    Message = $"Failed to retrieve mood record: {ex.Message}",
                    ErrorCode = "GET_EMOTIONALSTATE_ERROR",
                    Data = null
                };
            }
        }
    }
}