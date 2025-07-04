﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalTherapyBackendApp.Application.Dtos.Subscription;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionDto>> GetAllSubscriptionsAsync();
        Task<SubscriptionDto> GetSubscriptionByIdAsync(int id);
        Task<SubscriptionDto> GetSubscriptionByPlanIdAsync(string planId);
        Task<UserSubscriptionDto> GetUserActiveSubscriptionAsync(Guid userId);
        Task<SubscriptionDetailsDto> GetSubscriptionDetailsAsync(int subscriptionId, string countryCode, string languageCode);
        Task<UserSubscriptionDto> SubscribeUserAsync(Guid userId, int subscriptionId, string paymentMethod, decimal amount, string transactionId);
        Task<bool> CancelSubscriptionAsync(int userSubscriptionId);
        Task<bool> ToggleAutoRenewAsync(int userSubscriptionId, bool autoRenew);
        Task<List<PaymentDto>> GetUserPaymentHistoryAsync(Guid userId);
        Task<bool> CheckUserFeatureAccessAsync(Guid userId, string featureName);
        Task<int> GetUserFeatureLimitAsync(Guid userId, string featureName);
        Task<bool> ProcessPaymentWebhookAsync(string webhookData);
        Task<List<SubscriptionDetailsDto>> GetSubscriptionsWithDetailsAsync(string countryCode, string languageCode);
        Task<List<SubscriptionPriceDto>> GetSubscriptionPricesAsync(int subscriptionId);
        Task<List<SubscriptionTranslationDto>> GetSubscriptionTranslationsAsync(int subscriptionId);
        Task<bool> AddSubscriptionPriceAsync(SubscriptionPriceDto priceDto);
        Task<bool> AddSubscriptionTranslationAsync(SubscriptionTranslationDto translationDto);
        Task<bool> UpdateSubscriptionPriceAsync(SubscriptionPriceDto priceDto);
        Task<bool> UpdateSubscriptionTranslationAsync(SubscriptionTranslationDto translationDto);
        Task<bool> DeleteSubscriptionPriceAsync(int priceId);
        Task<bool> DeleteSubscriptionTranslationAsync(int translationId);
        Task<SubscriptionDto> AddSubscriptionPlanAsync(SubscriptionDto subscriptionDto);
        Task<SubscriptionDto> UpdateSubscriptionPlanAsync(SubscriptionDto subscriptionDto);
        Task<List<UserSubscriptionDto>> GetAllUserSubscriptionsAsync();
        Task<List<SubscriptionDetailsDto>> GetSubscriptionsWithDetailsByRoleAsync(string roleId, string countryCode, string languageCode);
        Task<int> GetFeatureUsageCountAsync(Guid userId, string featureName, string limitType = "daily");
        Task IncrementFeatureUsageAsync(Guid userId, string featureName, DateTime? usageTime = null);
        Task<bool> HasAdvancedFeatureAccessAsync(Guid userId, string featureName, int requiredLevel = 1);
        Task<FeatureLimitResult> CheckFeatureLimitAsync(Guid userId, string featureName, string limitType = "daily", DateTime? specificDate = null);
        Task MarkFeatureUsageAsDeletedAsync(Guid userId, string featureName, DateTime usageTime);
    }

    public class FeatureLimitResult
    {
        public bool IsAllowed { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public int CurrentUsage { get; set; }
        public int Limit { get; set; }
        public DateTime? ResetTime { get; set; }
        public DateTime? SpecificDate { get; set; }
    }
}