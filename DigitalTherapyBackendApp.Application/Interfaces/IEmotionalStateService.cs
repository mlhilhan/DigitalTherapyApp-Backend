using DigitalTherapyBackendApp.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public interface IEmotionalStateService
    {
        Task<EmotionalStateDto> GetByIdAsync(Guid id, Guid userId);
        Task<List<EmotionalStateDto>> GetAllByUserIdAsync(Guid userId);
        Task<List<EmotionalStateDto>> GetByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<List<EmotionalStateDto>> GetBookmarkedAsync(Guid userId);
        Task<Guid> CreateAsync(CreateEmotionalStateDto dto, Guid userId);
        Task<bool> UpdateAsync(Guid id, UpdateEmotionalStateDto dto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<bool> ToggleBookmarkAsync(Guid id, Guid userId);
        Task<EmotionalStateStatisticsDto> GetStatisticsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetActiveEntryCountForDateAsync(Guid userId, DateTime date);
    }
}
