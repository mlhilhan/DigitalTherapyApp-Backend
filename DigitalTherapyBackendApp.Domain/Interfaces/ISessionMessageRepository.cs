using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface ISessionMessageRepository
    {
        Task<SessionMessage> GetByIdAsync(Guid id);
        Task<IEnumerable<SessionMessage>> GetBySessionIdAsync(Guid sessionId);
        Task<IEnumerable<SessionMessage>> GetRecentBySessionIdAsync(Guid sessionId, int count = 50);
        Task<SessionMessage> AddAsync(SessionMessage message);
        Task<SessionMessage> AddUserMessageAsync(Guid sessionId, Guid userId, string content);
        Task<SessionMessage> AddAiMessageAsync(Guid sessionId, Guid userId, string content);
        Task DeleteAsync(Guid id);
        Task<int> GetMessageCountAsync(Guid sessionId);
        Task<SessionMessage> GetLastMessageAsync(Guid sessionId);
    }
}