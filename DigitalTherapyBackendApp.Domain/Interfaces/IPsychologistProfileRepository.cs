using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IPsychologistProfileRepository
    {
        Task<PsychologistProfile> GetByIdAsync(Guid id);
        Task<PsychologistProfile> GetByUserIdAsync(Guid userId);
        Task<List<PsychologistProfile>> GetAllAsync();
        Task<List<PsychologistProfile>> GetByInstitutionIdAsync(Guid institutionId);
        Task<List<PsychologistProfile>> GetBySpecialtyAsync(string specialty);
        Task<List<PsychologistProfile>> GetAvailablePsychologistsAsync();
        Task<PsychologistProfile> AddAsync(PsychologistProfile profile);
        Task<PsychologistProfile> UpdateAsync(PsychologistProfile profile);
        Task<bool> DeleteAsync(Guid id);
        Task ClearSpecialtiesAsync(Guid psychologistId);
        Task UpdateSpecialtiesAsync(Guid psychologistId, List<Specialty> specialties);
        Task UpdateAvailabilityAsync(Guid psychologistId, bool isAvailable, List<PsychologistAvailabilitySlot> availabilitySlots);
    }
}