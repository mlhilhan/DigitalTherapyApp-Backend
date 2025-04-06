using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface IDailyTipService
    {
        Task<List<DailyTipCategoryDto>> GetAllCategoriesAsync(string languageCode);
        Task<List<DailyTipDto>> GetAllTipsAsync(string languageCode);
        Task<List<DailyTipDto>> GetTipsByCategoryAsync(string categoryKey, string languageCode);
        Task<DailyTipDto> GetTipByIdAsync(int id, string languageCode);
        Task<DailyTipDto> GetTipOfTheDayAsync(string languageCode);
        Task<List<DailyTipDto>> GetBookmarkedTipsAsync(string languageCode);
        Task<bool> ToggleBookmarkAsync(int tipId);
        Task<DailyTipDto> CreateTipAsync(CreateDailyTipDto tipDto);
        Task<DailyTipCategoryDto> CreateCategoryAsync(CreateDailyTipCategoryDto categoryDto);
        Task<bool> UpdateTipAsync(UpdateDailyTipDto tipDto);
        Task<bool> UpdateCategoryAsync(UpdateDailyTipCategoryDto categoryDto);
        Task<bool> DeleteTipAsync(int id);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
