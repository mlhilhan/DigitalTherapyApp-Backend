using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IEmotionalStateRepository
    {
        Task<EmotionalState> GetByIdAsync(Guid id, Guid userId);
        Task<List<EmotionalState>> GetAllByUserIdAsync(Guid userId);
        Task<List<EmotionalState>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<List<EmotionalState>> GetBookmarkedAsync(Guid userId);
        Task<Guid> CreateAsync(EmotionalState emotionalState);
        Task<bool> UpdateAsync(EmotionalState emotionalState);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<bool> ToggleBookmarkAsync(Guid id, Guid userId);
        Task<Dictionary<DateTime, double>> GetAverageMoodByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetFactorFrequencyAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetEntryCountAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> CountActiveEntriesForDateAsync(Guid userId, DateTime date);
    }
}
