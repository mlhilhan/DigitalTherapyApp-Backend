using AutoMapper;
using DigitalTherapyBackendApp.Application.Dtos.Subscription;
using DigitalTherapyBackendApp.Application.Interfaces;
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
                    return userSubscription.Subscription.HasAdvancedReports;

                case "all_meditation_content":
                    return userSubscription.Subscription.HasAllMeditationContent;

                case "emergency_support":
                    return userSubscription.Subscription.HasEmergencySupport;

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
    }
}