using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries
{
    public class GetEmotionalStateByIdQuery : IRequest<EmotionalStateResponse>
    {
        public Guid EmotionalStateId { get; set; }
        public Guid UserId { get; set; }
    }


    public class GetEmotionalStateByIdQueryHandler : IRequestHandler<GetEmotionalStateByIdQuery, EmotionalStateResponse>
    {
        private readonly IEmotionalStateRepository _emotionalStateRepository;

        public GetEmotionalStateByIdQueryHandler(IEmotionalStateRepository emotionalStateRepository)
        {
            _emotionalStateRepository = emotionalStateRepository;
        }

        public async Task<EmotionalStateResponse> Handle(GetEmotionalStateByIdQuery request, CancellationToken cancellationToken)
        {
            var emotionalState = await _emotionalStateRepository.GetByIdAsync(request.EmotionalStateId);

            if (emotionalState == null || emotionalState.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to view this emotional state record.");

            return new EmotionalStateResponse
            {
                Id = emotionalState.Id,
                Mood = emotionalState.Mood,
                MoodIntensity = emotionalState.MoodIntensity,
                Notes = emotionalState.Notes,
                CreatedAt = emotionalState.CreatedAt
            };
        }
    }
}