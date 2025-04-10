using AutoMapper;
using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Entities.Subscriptions;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentGatewayService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            AppDbContext context,
            IPaymentGatewayService paymentService,
            IMapper mapper,
            ILogger<SubscriptionService> logger)
        {
            _context = context;
            _paymentService = paymentService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<SubscriptionDto>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _context.Subscriptions
                .Where(s => s.IsActive)
                .OrderBy(s => s.BasePrice)
                .ToListAsync();

            return _mapper.Map<List<SubscriptionDto>>(subscriptions);
        }

        public async Task<SubscriptionDto> GetSubscriptionByIdAsync(int id)
        {
            var subscription = await _context.Subscriptions
                .FindAsync(id);

            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<SubscriptionDto> GetSubscriptionByPlanIdAsync(string planId)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.PlanId == planId && s.IsActive);

            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<UserSubscriptionDto> GetUserActiveSubscriptionAsync(Guid userId)
        {
            var userSubscription = await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();

            return _mapper.Map<UserSubscriptionDto>(userSubscription);
        }

        public async Task<SubscriptionDetailsDto> GetSubscriptionDetailsAsync(int subscriptionId, string countryCode, string languageCode)
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.PricesByCountry.Where(p => p.CountryCode == countryCode))
                .Include(s => s.Translations.Where(t => t.LanguageCode == languageCode))
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription == null)
                return null;

            var subscriptionPrice = subscription.PricesByCountry.FirstOrDefault();
            var translation = subscription.Translations.FirstOrDefault();

            var details = new SubscriptionDetailsDto
            {
                Id = subscription.Id,
                PlanId = subscription.PlanId,
                Name = translation?.Name ?? subscription.Name,
                Description = translation?.Description ?? subscription.Description,
                Price = subscriptionPrice?.Price ?? subscription.BasePrice,
                CurrencyCode = subscriptionPrice?.CurrencyCode ?? subscription.BaseCurrency,
                CountryCode = countryCode,
                LanguageCode = languageCode,
                DurationInDays = subscription.DurationInDays,
                MoodEntryLimit = subscription.MoodEntryLimit,
                AIChatSessionsPerWeek = subscription.AIChatSessionsPerWeek,
                MessageLimitPerChat = subscription.MessageLimitPerChat,
                HasPsychologistSupport = subscription.HasPsychologistSupport,
                PsychologistSessionsPerMonth = subscription.PsychologistSessionsPerMonth,
                HasEmergencySupport = subscription.HasEmergencySupport,
                HasAdvancedReports = subscription.HasAdvancedReports,
                HasAllMeditationContent = subscription.HasAllMeditationContent
            };

            return details;
        }

        public async Task<UserSubscriptionDto> SubscribeUserAsync(Guid userId, int subscriptionId, string paymentMethod, decimal amount, string transactionId)
        {
            var activeSubscription = await _context.UserSubscriptions
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (activeSubscription != null)
            {
                activeSubscription.IsActive = false;
                activeSubscription.UpdatedAt = DateTime.UtcNow;
                _context.UserSubscriptions.Update(activeSubscription);
            }

            var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
            if (subscription == null)
                throw new Exception("No subscription plans found.");

            var userSubscription = new UserSubscription
            {
                UserId = userId,
                SubscriptionId = subscriptionId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(subscription.DurationInDays),
                IsActive = true,
                AutoRenew = true,
                TransactionId = transactionId,
                PaymentMethod = paymentMethod,
                PaidAmount = amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.UserSubscriptions.AddAsync(userSubscription);
            await _context.SaveChangesAsync();

            if (amount > 0 && !string.IsNullOrEmpty(transactionId))
            {
                var payment = new Payment
                {
                    UserSubscriptionId = userSubscription.Id,
                    TransactionId = transactionId,
                    Amount = amount,
                    Currency = "USD",
                    Status = "completed",
                    PaymentMethod = paymentMethod,
                    PaymentDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();
            }

            userSubscription.Subscription = subscription;

            return _mapper.Map<UserSubscriptionDto>(userSubscription);
        }

        public async Task<bool> CancelSubscriptionAsync(int userSubscriptionId)
        {
            var subscription = await _context.UserSubscriptions.FindAsync(userSubscriptionId);
            if (subscription == null)
                return false;

            subscription.IsActive = false;
            subscription.AutoRenew = false;
            subscription.UpdatedAt = DateTime.UtcNow;

            _context.UserSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleAutoRenewAsync(int userSubscriptionId, bool autoRenew)
        {
            var subscription = await _context.UserSubscriptions.FindAsync(userSubscriptionId);
            if (subscription == null)
                return false;

            subscription.AutoRenew = autoRenew;
            subscription.UpdatedAt = DateTime.UtcNow;

            _context.UserSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<PaymentDto>> GetUserPaymentHistoryAsync(Guid userId)
        {
            var payments = await _context.Payments
                .Include(p => p.UserSubscription)
                .Where(p => p.UserSubscription.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return _mapper.Map<List<PaymentDto>>(payments);
        }

        public async Task<bool> CheckUserFeatureAccessAsync(Guid userId, string featureName)
        {
            var userSubscription = await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();

            if (userSubscription == null)
                return false;

            switch (featureName.ToLower())
            {
                case "psychologist_support":
                    return userSubscription.Subscription.HasPsychologistSupport;

                case "advanced_reports":
                case "advanced_mood_views":
                    return userSubscription.Subscription.HasAdvancedReports;

                case "all_meditation_content":
                    return userSubscription.Subscription.HasAllMeditationContent;

                case "emergency_support":
                    return userSubscription.Subscription.HasEmergencySupport;

                case "mood_entry":
                case "basic_mood_tracking":
                    return true;

                case "basic_ai_chat":
                    return true;

                default:
                    return false;
            }
        }

        public async Task<int> GetUserFeatureLimitAsync(Guid userId, string featureName)
        {
            var userSubscription = await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();

            if (userSubscription == null)
                return 0;

            switch (featureName.ToLower())
            {
                case "mood_entry":
                    return userSubscription.Subscription.MoodEntryLimit;

                case "ai_chat_session":
                    return userSubscription.Subscription.AIChatSessionsPerWeek;

                case "chat_message":
                    return userSubscription.Subscription.MessageLimitPerChat;

                case "psychologist_session":
                    return userSubscription.Subscription.PsychologistSessionsPerMonth;

                default:
                    return 0;
            }
        }

        public async Task<bool> ProcessPaymentWebhookAsync(string webhookData)
        {
            try
            {
                var paymentResult = _paymentService.ParseWebhookData(webhookData);

                if (paymentResult.Success)
                {
                    var userSubscription = await _context.UserSubscriptions
                        .FirstOrDefaultAsync(us => us.TransactionId == paymentResult.TransactionId);

                    if (userSubscription != null)
                    {
                        var payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.TransactionId == paymentResult.TransactionId);

                        if (payment != null)
                        {
                            payment.Status = paymentResult.Status;
                            _context.Payments.Update(payment);
                            await _context.SaveChangesAsync();
                        }

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme webhook'u işlenirken hata oluştu");
                return false;
            }
        }

        public async Task<List<SubscriptionDetailsDto>> GetSubscriptionsWithDetailsAsync(string countryCode, string languageCode)
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.PricesByCountry.Where(p => p.CountryCode == countryCode))
                .Include(s => s.Translations.Where(t => t.LanguageCode == languageCode))
                .Where(s => s.IsActive)
                .OrderBy(s => s.BasePrice)
                .ToListAsync();

            var result = new List<SubscriptionDetailsDto>();

            foreach (var subscription in subscriptions)
            {
                var subscriptionPrice = subscription.PricesByCountry.FirstOrDefault();
                var translation = subscription.Translations.FirstOrDefault();

                result.Add(new SubscriptionDetailsDto
                {
                    Id = subscription.Id,
                    PlanId = subscription.PlanId,
                    Name = translation?.Name ?? subscription.Name,
                    Description = translation?.Description ?? subscription.Description,
                    Price = subscriptionPrice?.Price ?? subscription.BasePrice,
                    CurrencyCode = subscriptionPrice?.CurrencyCode ?? subscription.BaseCurrency,
                    CountryCode = countryCode,
                    LanguageCode = languageCode,
                    DurationInDays = subscription.DurationInDays,
                    MoodEntryLimit = subscription.MoodEntryLimit,
                    AIChatSessionsPerWeek = subscription.AIChatSessionsPerWeek,
                    MessageLimitPerChat = subscription.MessageLimitPerChat,
                    HasPsychologistSupport = subscription.HasPsychologistSupport,
                    PsychologistSessionsPerMonth = subscription.PsychologistSessionsPerMonth,
                    HasEmergencySupport = subscription.HasEmergencySupport,
                    HasAdvancedReports = subscription.HasAdvancedReports,
                    HasAllMeditationContent = subscription.HasAllMeditationContent
                });
            }

            return result;
        }

        public async Task<List<SubscriptionPriceDto>> GetSubscriptionPricesAsync(int subscriptionId)
        {
            var prices = await _context.SubscriptionPrices
                .Where(p => p.SubscriptionId == subscriptionId)
                .ToListAsync();

            return _mapper.Map<List<SubscriptionPriceDto>>(prices);
        }

        public async Task<List<SubscriptionTranslationDto>> GetSubscriptionTranslationsAsync(int subscriptionId)
        {
            var translations = await _context.SubscriptionTranslations
                .Where(t => t.SubscriptionId == subscriptionId)
                .ToListAsync();

            return _mapper.Map<List<SubscriptionTranslationDto>>(translations);
        }

        public async Task<bool> AddSubscriptionPriceAsync(SubscriptionPriceDto priceDto)
        {
            var price = _mapper.Map<SubscriptionPrice>(priceDto);

            await _context.SubscriptionPrices.AddAsync(price);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> AddSubscriptionTranslationAsync(SubscriptionTranslationDto translationDto)
        {
            var translation = _mapper.Map<SubscriptionTranslation>(translationDto);

            await _context.SubscriptionTranslations.AddAsync(translation);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> UpdateSubscriptionPriceAsync(SubscriptionPriceDto priceDto)
        {
            var price = await _context.SubscriptionPrices.FindAsync(priceDto.Id);
            if (price == null)
                return false;

            _mapper.Map(priceDto, price);
            _context.SubscriptionPrices.Update(price);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> UpdateSubscriptionTranslationAsync(SubscriptionTranslationDto translationDto)
        {
            var translation = await _context.SubscriptionTranslations.FindAsync(translationDto.Id);
            if (translation == null)
                return false;

            _mapper.Map(translationDto, translation);
            _context.SubscriptionTranslations.Update(translation);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteSubscriptionPriceAsync(int priceId)
        {
            var price = await _context.SubscriptionPrices.FindAsync(priceId);
            if (price == null)
                return false;

            _context.SubscriptionPrices.Remove(price);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteSubscriptionTranslationAsync(int translationId)
        {
            var translation = await _context.SubscriptionTranslations.FindAsync(translationId);
            if (translation == null)
                return false;

            _context.SubscriptionTranslations.Remove(translation);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<SubscriptionDto> AddSubscriptionPlanAsync(SubscriptionDto subscriptionDto)
        {
            var subscription = _mapper.Map<Subscription>(subscriptionDto);

            subscription.CreatedAt = DateTime.UtcNow;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<SubscriptionDto> UpdateSubscriptionPlanAsync(SubscriptionDto subscriptionDto)
        {
            var subscription = await _context.Subscriptions.FindAsync(subscriptionDto.Id);
            if (subscription == null)
                return null;

            _mapper.Map(subscriptionDto, subscription);
            subscription.UpdatedAt = DateTime.UtcNow;

            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<List<UserSubscriptionDto>> GetAllUserSubscriptionsAsync()
        {
            var userSubscriptions = await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Include(us => us.User)
                .OrderByDescending(us => us.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<UserSubscriptionDto>>(userSubscriptions);
        }

        public async Task<List<SubscriptionDetailsDto>> GetSubscriptionsWithDetailsByRoleAsync(string roleId, string countryCode, string languageCode)
        {
            var allPlans = await GetSubscriptionsWithDetailsAsync(countryCode, languageCode);

            if (roleId == "4b41d3bc-95cb-4758-8c01-c5487707931e") // Patient Role ID
            {
                return allPlans.Where(p => p.PlanId != "pro").ToList();
            }
            else if (roleId == "40c2b39a-a133-4ba9-a97b-ce351bd101ac") // Psychologist Role ID
            {
                // Psikolog rolü için özel planları döndür
                return allPlans.Where(p => p.PlanId != "free").ToList();
            }
            else if (roleId == "ce6c9f4d-8b26-4971-853a-69bafe48c012") // Admin Role ID
            {
                return allPlans;
            }
            else if (roleId == "5e6ef66e-8298-4002-b765-5a794f149362") // Institution Role ID
            {
                return allPlans.Where(p => p.PlanId == "pro").ToList();
            }

            return allPlans.Where(p => p.PlanId == "free").ToList();
        }

        public async Task<FeatureLimitResult> CheckFeatureLimitAsync(Guid userId, string featureName, string limitType = "daily", DateTime? specificDate = null)
        {
            var userSubscription = await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();

            int defaultLimit = 1;
            string planId = userSubscription?.Subscription?.PlanId ?? "free";

            if (userSubscription == null)
            {
                if (featureName.ToLower() == "mood_entry")
                {
                    return new FeatureLimitResult
                    {
                        IsAllowed = true,
                        Message = "Free plan allows 1 mood entry per day.",
                        ErrorCode = null,
                        CurrentUsage = 0,
                        Limit = defaultLimit,
                        ResetTime = DateTime.UtcNow.Date.AddDays(1),
                        SpecificDate = specificDate
                    };
                }

                return new FeatureLimitResult
                {
                    IsAllowed = false,
                    Message = "You don't have an active subscription.",
                    ErrorCode = "NO_ACTIVE_SUBSCRIPTION",
                    CurrentUsage = 0,
                    Limit = 0
                };
            }

            bool hasAccess = await CheckUserFeatureAccessAsync(userId, featureName);
            if (!hasAccess)
            {
                return new FeatureLimitResult
                {
                    IsAllowed = false,
                    Message = "Your subscription does not include access to this feature.",
                    ErrorCode = "FEATURE_ACCESS_DENIED",
                    CurrentUsage = 0,
                    Limit = 0
                };
            }

            int? limit = null;
            DateTime? resetTime = null;

            switch (featureName.ToLower())
            {
                case "mood_entry":
                    limit = userSubscription.Subscription.MoodEntryLimit;
                    resetTime = DateTime.UtcNow.Date.AddDays(1);
                    break;
                case "ai_chat_session":
                    limit = userSubscription.Subscription.AIChatSessionsPerWeek;
                    int daysToAdd = (8 - (int)DateTime.UtcNow.DayOfWeek) % 7;
                    if (daysToAdd == 0) daysToAdd = 7;
                    resetTime = DateTime.UtcNow.Date.AddDays(daysToAdd);
                    break;

                case "chat_message":
                    limit = userSubscription.Subscription.MessageLimitPerChat;
                    resetTime = null;
                    break;

                case "psychologist_session":
                    limit = userSubscription.Subscription.PsychologistSessionsPerMonth;
                    resetTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1);
                    break;

                default:
                    limit = 0;
                    resetTime = null;
                    break;
            }

            if (limit == null || limit <= 0)
            {
                if (featureName.ToLower() == "mood_entry")
                {
                    limit = defaultLimit;
                }
                else if (limit == -1)
                {
                    return new FeatureLimitResult
                    {
                        IsAllowed = true,
                        Message = "No limit for this feature.",
                        CurrentUsage = 0,
                        Limit = -1,
                        ResetTime = resetTime,
                        SpecificDate = specificDate
                    };
                }
            }

            int currentUsage;
            if (specificDate.HasValue && featureName.ToLower() == "mood_entry")
            {
                DateTime startOfDay = specificDate.Value.Date;
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                try
                {
                    currentUsage = await _context.FeatureUsages
                        .CountAsync(fu => fu.UserId == userId &&
                                    fu.FeatureName == featureName &&
                                    fu.UsageTime >= startOfDay &&
                                    fu.UsageTime <= endOfDay &&
                                    !fu.IsDeleted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error counting feature usage");
                    currentUsage = 0;
                }
            }
            else
            {
                currentUsage = await GetFeatureUsageCountAsync(userId, featureName, limitType);
            }

            bool isAllowed = currentUsage < limit;

            return new FeatureLimitResult
            {
                IsAllowed = isAllowed,
                Message = isAllowed
                    ? $"You have {limit - currentUsage} uses left."
                    : $"You have reached your daily limit for this feature.",
                ErrorCode = isAllowed ? null : "FEATURE_LIMIT_REACHED",
                CurrentUsage = currentUsage,
                Limit = limit.Value,
                ResetTime = resetTime,
                SpecificDate = specificDate
            };
        }

        public async Task<int> GetFeatureUsageCountAsync(Guid userId, string featureName, string limitType = "daily")
        {
            try
            {
                DateTime startDate;

                switch (limitType.ToLower())
                {
                    case "daily":
                        startDate = DateTime.UtcNow.Date;
                        break;
                    case "weekly":
                        int daysToSubtract = (int)DateTime.UtcNow.DayOfWeek - 1;
                        if (daysToSubtract < 0) daysToSubtract = 6;
                        startDate = DateTime.UtcNow.Date.AddDays(-daysToSubtract);
                        break;
                    case "monthly":
                        startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        break;
                    default:
                        startDate = DateTime.UtcNow.Date;
                        break;
                }

                var usageCount = await _context.FeatureUsages
                    .CountAsync(fu => fu.UserId == userId &&
                                fu.FeatureName == featureName &&
                                fu.UsageTime >= startDate);

                return usageCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feature usage count retrieval failed");
                return 0;
            }
        }

        public async Task IncrementFeatureUsageAsync(Guid userId, string featureName, DateTime? usageTime = null)
        {
            try
            {
                DateTime actualUsageTime = usageTime.HasValue
                    ? usageTime.Value.Date
                    : DateTime.UtcNow;

                var featureUsage = new FeatureUsage
                {
                    UserId = userId,
                    FeatureName = featureName,
                    UsageTime = actualUsageTime,
                    IsDeleted = false
                };

                await _context.FeatureUsages.AddAsync(featureUsage);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feature usage increment failed");
            }
        }

        public async Task<bool> HasAdvancedFeatureAccessAsync(Guid userId, string featureName, int requiredLevel = 1)
        {
            var userSubscription = await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();

            if (userSubscription == null)
                return requiredLevel <= 1;

            int userLevel;
            switch (userSubscription.Subscription.PlanId.ToLower())
            {
                case "free": userLevel = 1; break;
                case "standard": userLevel = 2; break;
                case "premium": userLevel = 3; break;
                case "pro": userLevel = 4; break;
                default: userLevel = 1; break;
            }

            if (userLevel < requiredLevel)
                return false;

            return await CheckUserFeatureAccessAsync(userId, featureName);
        }

        public async Task MarkFeatureUsageAsDeletedAsync(Guid userId, string featureName, DateTime usageTime)
        {
            try
            {
                DateTime startOfDay = usageTime.Date;
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                var featureUsages = await _context.FeatureUsages
                    .Where(fu => fu.UserId == userId &&
                           fu.FeatureName == featureName &&
                           fu.UsageTime >= startOfDay &&
                           fu.UsageTime <= endOfDay)
                    .ToListAsync();

                foreach (var featureUsage in featureUsages)
                {
                    featureUsage.IsDeleted = true;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Marking feature usage as deleted failed");
            }
        }
    }
}