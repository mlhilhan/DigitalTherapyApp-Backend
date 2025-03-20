using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands
{
    public class UpdateEmotionalStateCommand : IRequest<EmotionalStateResponse>
    {
        public Guid EmotionalStateId { get; set; }
        public Guid UserId { get; set; }
        public string Mood { get; set; }
        public int MoodIntensity { get; set; }
        public string Notes { get; set; }
    }


    public class UpdateEmotionalStateCommandHandler : IRequestHandler<UpdateEmotionalStateCommand, EmotionalStateResponse>
    {
        private readonly IEmotionalStateRepository _emotionalStateRepository;

        public UpdateEmotionalStateCommandHandler(IEmotionalStateRepository emotionalStateRepository)
        {
            _emotionalStateRepository = emotionalStateRepository;
        }

        public async Task<EmotionalStateResponse> Handle(UpdateEmotionalStateCommand request, CancellationToken cancellationToken)
        {
            var emotionalState = await _emotionalStateRepository.GetByIdAsync(request.EmotionalStateId);

            if (emotionalState == null || emotionalState.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to update this emotional state record.");

            emotionalState.Mood = request.Mood;
            emotionalState.MoodIntensity = request.MoodIntensity;
            emotionalState.Notes = request.Notes;

            await _emotionalStateRepository.UpdateAsync(emotionalState);

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