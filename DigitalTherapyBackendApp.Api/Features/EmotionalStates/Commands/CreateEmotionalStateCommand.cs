using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands
{
    public class CreateEmotionalStateCommand : IRequest<EmotionalStateResponse>
    {
        public Guid UserId { get; set; }
        public string Mood { get; set; }
        public int MoodIntensity { get; set; }
        public string Notes { get; set; }
    }


    public class CreateEmotionalStateCommandHandler : IRequestHandler<CreateEmotionalStateCommand, EmotionalStateResponse>
    {
        private readonly IEmotionalStateRepository _emotionalStateRepository;

        public CreateEmotionalStateCommandHandler(IEmotionalStateRepository emotionalStateRepository)
        {
            _emotionalStateRepository = emotionalStateRepository;
        }

        public async Task<EmotionalStateResponse> Handle(CreateEmotionalStateCommand request, CancellationToken cancellationToken)
        {
            var emotionalState = new EmotionalState
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Mood = request.Mood,
                MoodIntensity = request.MoodIntensity,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _emotionalStateRepository.AddAsync(emotionalState);

            return new EmotionalStateResponse
            {
                Id = result.Id,
                Mood = result.Mood,
                MoodIntensity = result.MoodIntensity,
                Notes = result.Notes,
                CreatedAt = result.CreatedAt
            };
        }
    }
}