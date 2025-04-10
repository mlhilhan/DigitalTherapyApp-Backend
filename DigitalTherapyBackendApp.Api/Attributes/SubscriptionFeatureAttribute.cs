using System;
using System.Threading.Tasks;
using DigitalTherapyBackendApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace DigitalTherapyBackendApp.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SubscriptionFeatureAttribute : ActionFilterAttribute
    {
        public string FeatureName { get; }
        public int RequiredLevel { get; }
        public string LimitType { get; }
        public bool CheckLimit { get; }
        public bool AutoIncrement { get; }

        public SubscriptionFeatureAttribute(
            string featureName,
            int requiredLevel = 1,
            bool checkLimit = false,
            string limitType = "daily",
            bool autoIncrement = false)
        {
            FeatureName = featureName;
            RequiredLevel = requiredLevel;
            CheckLimit = checkLimit;
            LimitType = limitType;
            AutoIncrement = autoIncrement;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var subscriptionService = context.HttpContext.RequestServices.GetRequiredService<ISubscriptionService>();

            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userIdGuid))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            bool hasAccess = await subscriptionService.HasAdvancedFeatureAccessAsync(userIdGuid, FeatureName, RequiredLevel);

            if (!hasAccess)
            {
                context.Result = new ObjectResult(new
                {
                    Success = false,
                    Message = "Your subscription plan does not include access to this feature.",
                    ErrorCode = "SUBSCRIPTION_FEATURE_RESTRICTED"
                })
                {
                    StatusCode = 403
                };
                return;
            }

            if (CheckLimit)
            {
                var limitResult = await subscriptionService.CheckFeatureLimitAsync(userIdGuid, FeatureName, LimitType);

                if (!limitResult.IsAllowed)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Success = false,
                        Message = limitResult.Message,
                        ErrorCode = limitResult.ErrorCode,
                        Data = new
                        {
                            CurrentUsage = limitResult.CurrentUsage,
                            Limit = limitResult.Limit,
                            ResetTime = limitResult.ResetTime
                        }
                    });
                    return;
                }
            }

            var result = await next();

            if (AutoIncrement && result.Exception == null && (result.Result == null || result.Result is ObjectResult obj && obj.StatusCode < 400))
            {
                await subscriptionService.IncrementFeatureUsageAsync(userIdGuid, FeatureName);
            }
        }
    }
}