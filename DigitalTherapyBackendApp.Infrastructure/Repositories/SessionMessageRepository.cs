using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class SessionMessageRepository : ISessionMessageRepository
    {
        private readonly AppDbContext _context;

        public SessionMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SessionMessage> GetByIdAsync(Guid id)
        {
            return await _context.SessionMessages
                .Include(m => m.Sender)
                .Include(m => m.Session)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<SessionMessage>> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.SessionMessages
                .Include(m => m.Sender)
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SessionMessage>> GetBySenderIdAsync(Guid senderId)
        {
            return await _context.SessionMessages
                .Include(m => m.Session)
                .Where(m => m.SenderId == senderId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<SessionMessage> AddAsync(SessionMessage sessionMessage)
        {
            await _context.SessionMessages.AddAsync(sessionMessage);
            await _context.SaveChangesAsync();
            return sessionMessage;
        }

        public async Task UpdateAsync(SessionMessage sessionMessage)
        {
            _context.SessionMessages.Update(sessionMessage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var sessionMessage = await _context.SessionMessages.FindAsync(id);
            if (sessionMessage != null)
            {
                _context.SessionMessages.Remove(sessionMessage);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SessionMessage>> GetSessionHistoryAsync(Guid sessionId, int limit, int offset)
        {
            return await _context.SessionMessages
                .Include(m => m.Sender)
                .Where(m => m.SessionId == sessionId)
                .OrderByDescending(m => m.SentAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }
}