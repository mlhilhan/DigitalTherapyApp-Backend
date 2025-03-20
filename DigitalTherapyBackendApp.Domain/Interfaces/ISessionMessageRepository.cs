using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface ISessionMessageRepository
    {
        Task<SessionMessage> GetByIdAsync(Guid id);
        Task<IEnumerable<SessionMessage>> GetBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<SessionMessage>> GetBySenderIdAsync(Guid senderId);
        Task<SessionMessage> AddAsync(SessionMessage sessionMessage);
        Task UpdateAsync(SessionMessage sessionMessage);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<SessionMessage>> GetSessionHistoryAsync(Guid sessionId, int limit, int offset);
    }
}
