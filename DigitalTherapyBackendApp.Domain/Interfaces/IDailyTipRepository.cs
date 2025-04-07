using DigitalTherapyBackendApp.Domain.Entities.DailyTip;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IDailyTipRepository
    {
        Task<List<DailyTipCategory>> GetAllCategoriesAsync();
        Task<List<DailyTip>> GetAllTipsAsync();
        Task<List<DailyTip>> GetTipsByCategoryAsync(string categoryKey);
        Task<DailyTip> GetTipByIdAsync(int id);
        Task<DailyTip> GetTipByKeyAsync(string tipKey);
        Task<DailyTip> GetTipOfTheDayAsync();
        Task<List<DailyTip>> GetBookmarkedTipsAsync();
        Task<bool> ToggleBookmarkAsync(int tipId);
        Task<DailyTip> CreateTipAsync(DailyTip tip);
        Task<DailyTipCategory> CreateCategoryAsync(DailyTipCategory category);
        Task<bool> UpdateTipAsync(DailyTip tip);
        Task<bool> UpdateCategoryAsync(DailyTipCategory category);
        Task<bool> DeleteTipAsync(int id);
        Task<bool> DeleteCategoryAsync(int id);
        Task<DailyTipCategory> GetCategoryByIdAsync(int id);
    }
}