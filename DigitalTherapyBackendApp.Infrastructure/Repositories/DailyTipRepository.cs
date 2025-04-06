using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class DailyTipRepository : IDailyTipRepository
    {
        private readonly AppDbContext _context;

        public DailyTipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DailyTipCategory>> GetAllCategoriesAsync()
        {
            return await _context.DailyTipCategories
                .Include(c => c.Translations)
                .ToListAsync();
        }

        public async Task<List<DailyTip>> GetAllTipsAsync()
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                .Include(t => t.Translations)
                .ToListAsync();
        }

        public async Task<List<DailyTip>> GetTipsByCategoryAsync(string categoryKey)
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                .Include(t => t.Translations)
                .Where(t => t.Category.CategoryKey == categoryKey)
                .ToListAsync();
        }

        public async Task<DailyTip> GetTipByIdAsync(int id)
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                .Include(t => t.Translations)
                .Include(t => t.Category.Translations)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<DailyTip> GetTipByKeyAsync(string tipKey)
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                .Include(t => t.Translations)
                .Include(t => t.Category.Translations)
                .FirstOrDefaultAsync(t => t.TipKey == tipKey);
        }

        public async Task<DailyTip> GetTipOfTheDayAsync()
        {
            // Günün ipucunu almak için basit bir implementasyon
            // Daha gelişmiş bir yöntem için günün tarihine göre bir algoritma kullanabilirsiniz
            return await _context.DailyTips
                .Include(t => t.Category)
                .Include(t => t.Translations)
                .Include(t => t.Category.Translations)
                .Where(t => t.IsFeatured)
                .OrderBy(t => t.Id) // Ya da CreatedAt
                .FirstOrDefaultAsync();
        }

        public async Task<List<DailyTip>> GetBookmarkedTipsAsync()
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                .Include(t => t.Translations)
                .Include(t => t.Category.Translations)
                .Where(t => t.IsBookmarked)
                .ToListAsync();
        }

        public async Task<bool> ToggleBookmarkAsync(int tipId)
        {
            var tip = await _context.DailyTips.FindAsync(tipId);
            if (tip == null) return false;

            tip.IsBookmarked = !tip.IsBookmarked;
            await _context.SaveChangesAsync();
            return tip.IsBookmarked;
        }

        public async Task<DailyTip> CreateTipAsync(DailyTip tip)
        {
            _context.DailyTips.Add(tip);
            await _context.SaveChangesAsync();
            return tip;
        }

        public async Task<DailyTipCategory> CreateCategoryAsync(DailyTipCategory category)
        {
            _context.DailyTipCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateTipAsync(DailyTip tip)
        {
            _context.DailyTips.Update(tip);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCategoryAsync(DailyTipCategory category)
        {
            _context.DailyTipCategories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTipAsync(int id)
        {
            var tip = await _context.DailyTips.FindAsync(id);
            if (tip == null) return false;

            _context.DailyTips.Remove(tip);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.DailyTipCategories.FindAsync(id);
            if (category == null) return false;

            _context.DailyTipCategories.Remove(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<DailyTipCategory> GetCategoryByIdAsync(int id)
        {
            return await _context.DailyTipCategories
                .Include(c => c.Translations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}