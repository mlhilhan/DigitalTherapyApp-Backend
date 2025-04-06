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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<DailyTip>> GetAllTipsAsync()
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                    .ThenInclude(c => c.Translations)
                .Include(t => t.Translations)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<DailyTip>> GetTipsByCategoryAsync(string categoryKey)
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                    .ThenInclude(c => c.Translations)
                .Include(t => t.Translations)
                .Where(t => t.Category.CategoryKey == categoryKey)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DailyTip> GetTipByIdAsync(int id)
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                    .ThenInclude(c => c.Translations)
                .Include(t => t.Translations)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<DailyTip> GetTipByKeyAsync(string tipKey)
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                    .ThenInclude(c => c.Translations)
                .Include(t => t.Translations)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TipKey == tipKey);
        }

        public async Task<DailyTip> GetTipOfTheDayAsync()
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                    .ThenInclude(c => c.Translations)
                .Include(t => t.Translations)
                .Where(t => t.IsFeatured)
                .OrderBy(t => t.CreatedAt)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<DailyTip>> GetBookmarkedTipsAsync()
        {
            return await _context.DailyTips
                .Include(t => t.Category)
                    .ThenInclude(c => c.Translations)
                .Include(t => t.Translations)
                .Where(t => t.IsBookmarked)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ToggleBookmarkAsync(int tipId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tip = await _context.DailyTips.FindAsync(tipId);
                if (tip == null) return false;

                tip.IsBookmarked = !tip.IsBookmarked;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return tip.IsBookmarked;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DailyTip> CreateTipAsync(DailyTip tip)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DailyTips.Add(tip);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return tip;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DailyTipCategory> CreateCategoryAsync(DailyTipCategory category)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DailyTipCategories.Add(category);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return category;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateTipAsync(DailyTip tip)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DailyTips.Update(tip);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(DailyTipCategory category)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DailyTipCategories.Update(category);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DeleteTipAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tip = await _context.DailyTips.FindAsync(id);
                if (tip == null) return false;

                _context.DailyTips.Remove(tip);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var category = await _context.DailyTipCategories.FindAsync(id);
                if (category == null) return false;

                _context.DailyTipCategories.Remove(category);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<DailyTipCategory> GetCategoryByIdAsync(int id)
        {
            return await _context.DailyTipCategories
                .Include(c => c.Translations)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}