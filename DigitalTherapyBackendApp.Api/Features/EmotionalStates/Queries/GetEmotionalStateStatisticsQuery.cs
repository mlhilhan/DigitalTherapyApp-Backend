using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Commands;
using DigitalTherapyBackendApp.Api.Features.EmotionalStates.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.EmotionalStates.Queries
{
    public class GetEmotionalStateStatisticsQuery : IRequest<EmotionalStateStatisticsResponse>
    {
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class GetEmotionalStateStatisticsQueryHandler : IRequestHandler<GetEmotionalStateStatisticsQuery, EmotionalStateStatisticsResponse>
    {
        private readonly IEmotionalStateRepository _emotionalStateRepository;

        public GetEmotionalStateStatisticsQueryHandler(IEmotionalStateRepository emotionalStateRepository)
        {
            _emotionalStateRepository = emotionalStateRepository;
        }

        public async Task<EmotionalStateStatisticsResponse> Handle(GetEmotionalStateStatisticsQuery request, CancellationToken cancellationToken)
        {
            // Ortalama duygu yoğunluğunu al
            var averageMoodIntensity = await _emotionalStateRepository.GetAverageMoodIntensityAsync(
                request.UserId,
                request.StartDate,
                request.EndDate);

            // Duygu frekanslarını al
            var moodFrequency = await _emotionalStateRepository.GetMoodFrequencyAsync(
                request.UserId,
                request.StartDate,
                request.EndDate);

            // En son eklenen duygu durumunu al
            var latestEmotionalStates = await _emotionalStateRepository.GetLatestEmotionalStatesAsync(request.UserId, 1);
            var mostRecentMood = latestEmotionalStates.FirstOrDefault();

            return new EmotionalStateStatisticsResponse
            {
                AverageMoodIntensity = averageMoodIntensity,
                MoodFrequency = moodFrequency,
                MostRecentMood = mostRecentMood != null
                    ? new EmotionalStateResponse
                    {
                        Id = mostRecentMood.Id,
                        Mood = mostRecentMood.Mood,
                        MoodIntensity = mostRecentMood.MoodIntensity,
                        Notes = mostRecentMood.Notes,
                        CreatedAt = mostRecentMood.CreatedAt
                    }
                    : null,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
        }
    }
}