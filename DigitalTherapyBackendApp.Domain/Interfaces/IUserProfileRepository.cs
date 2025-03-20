using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetByIdAsync(Guid id);
        Task<UserProfile> GetByUserIdAsync(Guid userId);
        Task<UserProfile> AddAsync(UserProfile userProfile);
        Task UpdateAsync(UserProfile userProfile);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}
