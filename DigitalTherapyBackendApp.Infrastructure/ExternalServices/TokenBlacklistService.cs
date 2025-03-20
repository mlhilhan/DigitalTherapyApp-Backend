using DigitalTherapyBackendApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace DigitalTherapyBackendApp.Infrastructure.ExternalServices
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly StackExchange.Redis.IDatabase _redisDatabase;

        public TokenBlacklistService(IConnectionMultiplexer redis)
        {
            _redisDatabase = redis.GetDatabase();
        }

        public async Task BlacklistTokenAsync(string token)
        {
            // Add Token in Blacklist And Remove
            await _redisDatabase.StringSetAsync(token, "blacklisted", TimeSpan.FromDays(1));
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            // Token Control in Blacklist
            return await _redisDatabase.KeyExistsAsync(token);
        }
    }
}
