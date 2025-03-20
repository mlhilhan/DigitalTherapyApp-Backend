using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface IRedisService
    {
        Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string> GetAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<bool> DeleteAsync(string key);
    }
}
