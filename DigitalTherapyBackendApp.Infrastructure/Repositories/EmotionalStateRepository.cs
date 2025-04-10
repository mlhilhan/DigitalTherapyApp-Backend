using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class EmotionalStateRepository : IEmotionalStateRepository
    {
        private readonly AppDbContext _context;

        public EmotionalStateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmotionalState> GetByIdAsync(Guid id, Guid userId)
        {
            return await _context.EmotionalStates
                .Where(e => e.Id == id && e.UserId == userId && !e.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmotionalState>> GetAllByUserIdAsync(Guid userId)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId && !e.IsDeleted)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<List<EmotionalState>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<List<EmotionalState>> GetBookmarkedAsync(Guid userId)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId && !e.IsDeleted && e.IsBookmarked)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<Guid> CreateAsync(EmotionalState emotionalState)
        {
            _context.EmotionalStates.Add(emotionalState);
            await _context.SaveChangesAsync();
            return emotionalState.Id;
        }

        public async Task<bool> UpdateAsync(EmotionalState emotionalState)
        {
            _context.EmotionalStates.Update(emotionalState);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var entity = await GetByIdAsync(id, userId);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleBookmarkAsync(Guid id, Guid userId)
        {
            var entity = await GetByIdAsync(id, userId);
            if (entity == null) return false;

            entity.IsBookmarked = !entity.IsBookmarked;
            entity.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Dictionary<DateTime, double>> GetAverageMoodByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate)
                .GroupBy(e => e.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AverageMood = g.Average(e => e.MoodLevel)
                })
                .ToDictionaryAsync(x => x.Date, x => x.AverageMood);
        }

        public async Task<Dictionary<string, int>> GetFactorFrequencyAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.EmotionalStates
                .Where(e => e.UserId == userId && !e.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            var entries = await query.ToListAsync();

            var factorFrequency = new Dictionary<string, int>();

            foreach (var entry in entries)
            {
                if (entry.Factors != null)
                {
                    foreach (var factor in entry.Factors)
                    {
                        if (factorFrequency.ContainsKey(factor))
                            factorFrequency[factor]++;
                        else
                            factorFrequency[factor] = 1;
                    }
                }
            }

            return factorFrequency;
        }

        public async Task<int> GetEntryCountAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.EmotionalStates
                .Where(e => e.UserId == userId && !e.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<int> CountActiveEntriesForDateAsync(Guid userId, DateTime date)
        {
            DateTime startOfDay = date.Date;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            return await _context.EmotionalStates
                .CountAsync(es =>
                    es.UserId == userId &&
                    es.Date >= startOfDay &&
                    es.Date <= endOfDay &&
                    !es.IsDeleted);
        }
    }
}