using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IPsychologistProfileRepository
    {
        Task<IEnumerable<PsychologistProfile>> GetAllAsync();
        Task<PsychologistProfile> GetByIdAsync(Guid id);
        Task<PsychologistProfile> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<PsychologistProfile>> GetByInstitutionIdAsync(Guid institutionId);
        Task<IEnumerable<PsychologistProfile>> GetIndependentPsychologistsAsync();
        Task<PsychologistProfile> AddAsync(PsychologistProfile psychologistProfile);
        Task<PsychologistProfile> UpdateAsync(PsychologistProfile psychologistProfile);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> AssignToInstitutionAsync(Guid psychologistId, Guid institutionId);
        Task<bool> RemoveFromInstitutionAsync(Guid psychologistId);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}
