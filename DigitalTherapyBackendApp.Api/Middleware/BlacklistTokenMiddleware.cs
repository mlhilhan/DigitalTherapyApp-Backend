using DigitalTherapyBackendApp.Application.Interfaces;

namespace DigitalTherapyBackendApp.Api.Middleware
{
    public class BlacklistTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public BlacklistTokenMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var tokenBlacklistService = scope.ServiceProvider.GetRequiredService<ITokenBlacklistService>();
                    var isBlacklisted = await tokenBlacklistService.IsTokenBlacklistedAsync(token);

                    if (isBlacklisted)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is blacklisted.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
