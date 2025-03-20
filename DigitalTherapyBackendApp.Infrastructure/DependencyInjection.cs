using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Identity;
using DigitalTherapyBackendApp.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Repositories;

namespace DigitalTherapyBackendApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
            services.AddScoped<IUserRepository, UserRepository>();

            var jwtSettings = configuration.GetSection("JWTSecurity");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            services.AddSingleton<TokenValidationParameters>(new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = "DigitalTherapyApp",
                ValidateAudience = true,
                ValidAudience = "DigitalTherapyUsers",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmotionalStateRepository, EmotionalStateRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<ITherapySessionRepository, TherapySessionRepository>();
            services.AddScoped<ISessionMessageRepository, SessionMessageRepository>();

            return services;
        }
    }
}
