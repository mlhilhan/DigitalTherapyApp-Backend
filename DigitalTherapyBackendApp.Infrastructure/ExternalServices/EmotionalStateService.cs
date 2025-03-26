using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public class EmotionalStateService : IEmotionalStateService
    {
        private readonly IEmotionalStateRepository _emotionalStateRepository;

        public EmotionalStateService(IEmotionalStateRepository emotionalStateRepository)
        {
            _emotionalStateRepository = emotionalStateRepository;
        }

        public async Task<EmotionalStateDto> GetByIdAsync(Guid id, Guid userId)
        {
            var entity = await _emotionalStateRepository.GetByIdAsync(id, userId);
            return entity != null ? MapToDto(entity) : null;
        }

        public async Task<List<EmotionalStateDto>> GetAllByUserIdAsync(Guid userId)
        {
            var entities = await _emotionalStateRepository.GetAllByUserIdAsync(userId);
            return entities.Select(MapToDto).ToList();
        }

        public async Task<List<EmotionalStateDto>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var entities = await _emotionalStateRepository.GetByDateRangeAsync(userId, startDate, endDate);
            return entities.Select(MapToDto).ToList();
        }

        public async Task<List<EmotionalStateDto>> GetBookmarkedAsync(Guid userId)
        {
            var entities = await _emotionalStateRepository.GetBookmarkedAsync(userId);
            return entities.Select(MapToDto).ToList();
        }

        public async Task<Guid> CreateAsync(CreateEmotionalStateDto dto, Guid userId)
        {
            var entity = new EmotionalState
            {
                UserId = userId,
                MoodLevel = dto.MoodLevel,
                Factors = dto.Factors,
                Notes = dto.Notes,
                Date = dto.Date,
                IsBookmarked = dto.IsBookmarked,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            return await _emotionalStateRepository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateEmotionalStateDto dto, Guid userId)
        {
            var entity = await _emotionalStateRepository.GetByIdAsync(id, userId);
            if (entity == null) return false;

            entity.MoodLevel = dto.MoodLevel;
            entity.Factors = dto.Factors;
            entity.Notes = dto.Notes;
            entity.Date = dto.Date;
            entity.IsBookmarked = dto.IsBookmarked;
            entity.UpdatedAt = DateTime.UtcNow;

            return await _emotionalStateRepository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            return await _emotionalStateRepository.DeleteAsync(id, userId);
        }

        public async Task<bool> ToggleBookmarkAsync(Guid id, Guid userId)
        {
            return await _emotionalStateRepository.ToggleBookmarkAsync(id, userId);
        }

        public async Task<EmotionalStateStatisticsDto> GetStatisticsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Varsayılan tarih aralığı: son 30 gün
            var effectiveStartDate = startDate ?? DateTime.UtcNow.AddDays(-30);
            var effectiveEndDate = endDate ?? DateTime.UtcNow;

            // Ortalama ruh hali değerlerini al
            var moodsByDate = await _emotionalStateRepository.GetAverageMoodByDateRangeAsync(
                userId, effectiveStartDate, effectiveEndDate);

            // Faktör frekanslarını al
            var factorFrequency = await _emotionalStateRepository.GetFactorFrequencyAsync(
                userId, effectiveStartDate, effectiveEndDate);

            // Toplam kayıt sayısını al
            var totalEntries = await _emotionalStateRepository.GetEntryCountAsync(
                userId, effectiveStartDate, effectiveEndDate);

            // Günlük ruh hali istatistiklerini oluştur
            var dailyMoods = moodsByDate.Select(kvp => new DailyMoodDto
            {
                Date = kvp.Key,
                AverageMood = kvp.Value,
                EntryCount = 1
            }).ToList();

            // Ortalama ruh hali değerini hesapla
            double averageMood = 0;
            if (moodsByDate.Count > 0)
            {
                averageMood = moodsByDate.Values.Average();
            }

            return new EmotionalStateStatisticsDto
            {
                AverageMood = averageMood,
                FactorFrequency = factorFrequency,
                DailyMoods = dailyMoods,
                TotalEntries = totalEntries
            };
        }


        private EmotionalStateDto MapToDto(EmotionalState entity)
        {
            return new EmotionalStateDto
            {
                Id = entity.Id,
                MoodLevel = entity.MoodLevel,
                Factors = entity.Factors,
                Notes = entity.Notes,
                Date = entity.Date,
                IsBookmarked = entity.IsBookmarked
            };
        }
    }
}
