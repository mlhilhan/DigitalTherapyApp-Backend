using DigitalTherapyBackendApp.Application.Dtos.DailyTips;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities.DailyTip;
using DigitalTherapyBackendApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public class DailyTipService : IDailyTipService
    {
        private readonly IDailyTipRepository _dailyTipRepository;

        public DailyTipService(IDailyTipRepository dailyTipRepository)
        {
            _dailyTipRepository = dailyTipRepository;
        }

        public async Task<List<DailyTipCategoryDto>> GetAllCategoriesAsync(string languageCode)
        {
            var categories = await _dailyTipRepository.GetAllCategoriesAsync();
            return categories.Select(c => MapCategoryToDto(c, languageCode)).ToList();
        }

        public async Task<List<DailyTipDto>> GetAllTipsAsync(string languageCode)
        {
            var tips = await _dailyTipRepository.GetAllTipsAsync();
            return tips.Select(t => MapTipToDto(t, languageCode)).ToList();
        }

        public async Task<List<DailyTipDto>> GetTipsByCategoryAsync(string categoryKey, string languageCode)
        {
            var tips = await _dailyTipRepository.GetTipsByCategoryAsync(categoryKey);
            return tips.Select(t => MapTipToDto(t, languageCode)).ToList();
        }

        public async Task<DailyTipDto> GetTipByIdAsync(int id, string languageCode)
        {
            var tip = await _dailyTipRepository.GetTipByIdAsync(id);
            return tip != null ? MapTipToDto(tip, languageCode) : null;
        }

        public async Task<DailyTipDto> GetTipOfTheDayAsync(string languageCode)
        {
            var tip = await _dailyTipRepository.GetTipOfTheDayAsync();
            return tip != null ? MapTipToDto(tip, languageCode) : null;
        }

        public async Task<List<DailyTipDto>> GetBookmarkedTipsAsync(string languageCode)
        {
            var tips = await _dailyTipRepository.GetBookmarkedTipsAsync();
            return tips.Select(t => MapTipToDto(t, languageCode)).ToList();
        }

        public async Task<bool> ToggleBookmarkAsync(int tipId)
        {
            return await _dailyTipRepository.ToggleBookmarkAsync(tipId);
        }

        public async Task<DailyTipDto> CreateTipAsync(CreateDailyTipDto tipDto)
        {
            var tip = new DailyTip
            {
                CategoryId = tipDto.CategoryId,
                TipKey = tipDto.TipKey,
                Icon = tipDto.Icon,
                Color = tipDto.Color,
                IsFeatured = tipDto.IsFeatured,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Translations = new List<DailyTipTranslation>()
            };

            foreach (var translation in tipDto.Translations)
            {
                tip.Translations.Add(new DailyTipTranslation
                {
                    LanguageCode = translation.Key,
                    Title = translation.Value.Title,
                    ShortDescription = translation.Value.ShortDescription,
                    Content = translation.Value.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            var createdTip = await _dailyTipRepository.CreateTipAsync(tip);
            return MapTipToDto(createdTip, tipDto.Translations.Keys.FirstOrDefault() ?? "en");
        }

        public async Task<DailyTipCategoryDto> CreateCategoryAsync(CreateDailyTipCategoryDto categoryDto)
        {
            var category = new DailyTipCategory
            {
                CategoryKey = categoryDto.CategoryKey,
                Icon = categoryDto.Icon,
                Color = categoryDto.Color,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Translations = new List<DailyTipCategoryTranslation>()
            };

            foreach (var translation in categoryDto.Translations)
            {
                category.Translations.Add(new DailyTipCategoryTranslation
                {
                    LanguageCode = translation.Key,
                    Name = translation.Value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            var createdCategory = await _dailyTipRepository.CreateCategoryAsync(category);
            return MapCategoryToDto(createdCategory, categoryDto.Translations.Keys.FirstOrDefault() ?? "en");
        }

        public async Task<bool> UpdateTipAsync(UpdateDailyTipDto tipDto)
        {
            var tip = await _dailyTipRepository.GetTipByIdAsync(tipDto.Id);
            if (tip == null) return false;

            tip.CategoryId = tipDto.CategoryId;
            tip.Icon = tipDto.Icon;
            tip.Color = tipDto.Color;
            tip.IsFeatured = tipDto.IsFeatured;
            tip.UpdatedAt = DateTime.UtcNow;

            foreach (var translation in tipDto.Translations)
            {
                var existingTranslation = tip.Translations.FirstOrDefault(t => t.LanguageCode == translation.Key);
                if (existingTranslation != null)
                {
                    existingTranslation.Title = translation.Value.Title;
                    existingTranslation.ShortDescription = translation.Value.ShortDescription;
                    existingTranslation.Content = translation.Value.Content;
                    existingTranslation.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    tip.Translations.Add(new DailyTipTranslation
                    {
                        LanguageCode = translation.Key,
                        Title = translation.Value.Title,
                        ShortDescription = translation.Value.ShortDescription,
                        Content = translation.Value.Content,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return await _dailyTipRepository.UpdateTipAsync(tip);
        }

        public async Task<bool> UpdateCategoryAsync(UpdateDailyTipCategoryDto categoryDto)
        {
            var category = await _dailyTipRepository.GetCategoryByIdAsync(categoryDto.Id);
            if (category == null) return false;

            category.Icon = categoryDto.Icon;
            category.Color = categoryDto.Color;
            category.UpdatedAt = DateTime.UtcNow;

            foreach (var translation in categoryDto.Translations)
            {
                var existingTranslation = category.Translations.FirstOrDefault(t => t.LanguageCode == translation.Key);
                if (existingTranslation != null)
                {
                    existingTranslation.Name = translation.Value;
                    existingTranslation.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    category.Translations.Add(new DailyTipCategoryTranslation
                    {
                        LanguageCode = translation.Key,
                        Name = translation.Value,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return await _dailyTipRepository.UpdateCategoryAsync(category);
        }

        public async Task<bool> DeleteTipAsync(int id)
        {
            return await _dailyTipRepository.DeleteTipAsync(id);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _dailyTipRepository.DeleteCategoryAsync(id);
        }


        #region Helper Methods
        private DailyTipDto MapTipToDto(DailyTip tip, string languageCode)
        {
            var translation = tip.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
                           ?? tip.Translations.FirstOrDefault();

            var categoryTranslation = tip.Category?.Translations
                .FirstOrDefault(t => t.LanguageCode == languageCode)
                ?? tip.Category?.Translations.FirstOrDefault();

            return new DailyTipDto
            {
                Id = tip.Id,
                TipKey = tip.TipKey,
                Title = translation?.Title ?? "Unknown Title",
                ShortDescription = translation?.ShortDescription ?? "",
                Content = translation?.Content ?? "",
                Icon = tip.Icon,
                Color = tip.Color,
                IsFeatured = tip.IsFeatured,
                IsBookmarked = tip.IsBookmarked,
                Category = tip.Category != null ? new DailyTipCategoryDto
                {
                    Id = tip.Category.Id,
                    CategoryKey = tip.Category.CategoryKey,
                    Name = categoryTranslation?.Name ?? "Unknown Category",
                    Icon = tip.Category.Icon,
                    Color = tip.Category.Color
                } : null
            };
        }

        private DailyTipCategoryDto MapCategoryToDto(DailyTipCategory category, string languageCode)
        {
            var translation = category.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
                           ?? category.Translations.FirstOrDefault();

            return new DailyTipCategoryDto
            {
                Id = category.Id,
                CategoryKey = category.CategoryKey,
                Name = translation?.Name ?? "Unknown Category",
                Icon = category.Icon,
                Color = category.Color,
                Tips = category.Tips?.Select(t => MapTipToDto(t, languageCode)).ToList()
            };
        }

        #endregion
    }
}
