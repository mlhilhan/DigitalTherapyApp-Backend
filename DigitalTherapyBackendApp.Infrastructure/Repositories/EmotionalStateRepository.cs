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

        public async Task<EmotionalState> GetByIdAsync(Guid id)
        {
            return await _context.EmotionalStates
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<EmotionalState>> GetByUserIdAsync(Guid userId)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmotionalState>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId && e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmotionalState> AddAsync(EmotionalState emotionalState)
        {
            await _context.EmotionalStates.AddAsync(emotionalState);
            await _context.SaveChangesAsync();
            return emotionalState;
        }

        public async Task UpdateAsync(EmotionalState emotionalState)
        {
            _context.EmotionalStates.Update(emotionalState);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var emotionalState = await _context.EmotionalStates.FindAsync(id);
            if (emotionalState != null)
            {
                _context.EmotionalStates.Remove(emotionalState);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<double> GetAverageMoodIntensityAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId && e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                .AverageAsync(e => e.MoodIntensity);
        }

        public async Task<List<EmotionalState>> GetLatestEmotionalStatesAsync(Guid userId, int count)
        {
            return await _context.EmotionalStates
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetMoodFrequencyAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var moodCounts = await _context.EmotionalStates
                .Where(e => e.UserId == userId && e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                .GroupBy(e => e.Mood)
                .Select(g => new { Mood = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Mood, x => x.Count);

            return moodCounts;
        }
    }
}