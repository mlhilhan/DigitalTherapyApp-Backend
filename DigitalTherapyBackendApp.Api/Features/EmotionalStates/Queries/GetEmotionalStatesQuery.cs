using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using System;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries
{
    public class GetEmotionalStatesQuery : IRequest<EmotionalStateListResponse>
    {
        public Guid UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }


    public class GetEmotionalStatesQueryHandler : IRequestHandler<GetEmotionalStatesQuery, EmotionalStateListResponse>
    {
        private readonly IEmotionalStateRepository _emotionalStateRepository;

        public GetEmotionalStatesQueryHandler(IEmotionalStateRepository emotionalStateRepository)
        {
            _emotionalStateRepository = emotionalStateRepository;
        }

        public async Task<EmotionalStateListResponse> Handle(GetEmotionalStatesQuery request, CancellationToken cancellationToken)
        {
            var emotionalStates = request.StartDate.HasValue && request.EndDate.HasValue
                ? await _emotionalStateRepository.GetByUserIdAndDateRangeAsync(
                    request.UserId,
                    request.StartDate.Value,
                    request.EndDate.Value)
                : await _emotionalStateRepository.GetByUserIdAsync(request.UserId);

            return new EmotionalStateListResponse
            {
                EmotionalStates = emotionalStates.Select(e => new EmotionalStateResponse
                {
                    Id = e.Id,
                    Mood = e.Mood,
                    MoodIntensity = e.MoodIntensity,
                    Notes = e.Notes,
                    CreatedAt = e.CreatedAt
                }).ToList()
            };
        }
    }
}