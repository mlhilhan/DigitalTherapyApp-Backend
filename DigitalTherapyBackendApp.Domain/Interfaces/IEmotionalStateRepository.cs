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
        Task<EmotionalState> GetByIdAsync(Guid id);
        Task<IEnumerable<EmotionalState>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<EmotionalState>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<EmotionalState> AddAsync(EmotionalState emotionalState);
        Task UpdateAsync(EmotionalState emotionalState);
        Task DeleteAsync(Guid id);
        Task<double> GetAverageMoodIntensityAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<List<EmotionalState>> GetLatestEmotionalStatesAsync(Guid userId, int count);
        Task<Dictionary<string, int>> GetMoodFrequencyAsync(Guid userId, DateTime startDate, DateTime endDate);
    }
}
