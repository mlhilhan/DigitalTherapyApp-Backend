using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IPatientProfileRepository
    {
        Task<IEnumerable<PatientProfile>> GetAllAsync();
        Task<PatientProfile> GetByIdAsync(Guid id);
        Task<PatientProfile> GetByUserIdAsync(Guid userId);
        Task<PatientProfile> AddAsync(PatientProfile patientProfile);
        Task<PatientProfile> UpdateAsync(PatientProfile patientProfile);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}
