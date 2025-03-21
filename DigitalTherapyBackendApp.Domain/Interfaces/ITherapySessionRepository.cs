using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface ITherapySessionRepository
    {
        Task<TherapySession> GetByIdAsync(Guid id);
        Task<IEnumerable<TherapySession>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<TherapySession>> GetByTherapistIdAsync(Guid therapistId);
        Task<IEnumerable<TherapySession>> GetActiveSessionsAsync(Guid userId);
        Task<IEnumerable<TherapySession>> GetAiSessionsByPatientIdAsync(Guid patientId);
        Task<IEnumerable<TherapySession>> GetHumanSessionsByPatientIdAsync(Guid patientId);
        Task<IEnumerable<TherapySession>> GetSessionsByRelationshipIdAsync(Guid relationshipId);
        Task<IEnumerable<TherapySession>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TherapySession>> GetCompletedSessionsAsync(Guid userId, bool isPatient);
        Task<TherapySession> AddAsync(TherapySession therapySession);
        Task UpdateAsync(TherapySession therapySession);
        Task DeleteAsync(Guid id);
        Task<int> GetSessionCountAsync(Guid userId, bool isPatient, DateTime startDate, DateTime endDate);
        Task<TherapySession> GetLastSessionAsync(Guid patientId, bool isAiSession);
        Task<bool> UpdateSessionStatusAsync(Guid id, string status);
    }
}
