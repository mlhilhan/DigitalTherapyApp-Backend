using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface ITherapistPatientRelationshipRepository
    {
        Task<IEnumerable<TherapistPatientRelationship>> GetAllAsync();
        Task<TherapistPatientRelationship> GetByIdAsync(Guid id);
        Task<IEnumerable<TherapistPatientRelationship>> GetByPsychologistIdAsync(Guid psychologistId);
        Task<IEnumerable<TherapistPatientRelationship>> GetByPatientIdAsync(Guid patientId);
        Task<TherapistPatientRelationship> GetActiveBetweenPsychologistAndPatientAsync(Guid psychologistId, Guid patientId);
        Task<IEnumerable<TherapistPatientRelationship>> GetByStatusAsync(string status);
        Task<TherapistPatientRelationship> AddAsync(TherapistPatientRelationship relationship);
        Task<TherapistPatientRelationship> UpdateAsync(TherapistPatientRelationship relationship);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid psychologistId, Guid patientId, string status);
        Task<bool> TerminateRelationshipAsync(Guid id, string notes = null);
    }
}
