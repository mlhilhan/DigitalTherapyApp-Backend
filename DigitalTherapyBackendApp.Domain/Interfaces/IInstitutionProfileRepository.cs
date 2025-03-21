using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IInstitutionProfileRepository
    {
        Task<IEnumerable<InstitutionProfile>> GetAllAsync();
        Task<InstitutionProfile> GetByIdAsync(Guid id);
        Task<InstitutionProfile> GetByUserIdAsync(Guid userId);
        Task<InstitutionProfile> GetWithPsychologistsAsync(Guid id);
        Task<IEnumerable<InstitutionProfile>> GetVerifiedInstitutionsAsync();
        Task<InstitutionProfile> AddAsync(InstitutionProfile institutionProfile);
        Task<InstitutionProfile> UpdateAsync(InstitutionProfile institutionProfile);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> VerifyInstitutionAsync(Guid id, bool verified);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}
