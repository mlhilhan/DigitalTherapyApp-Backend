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

        public async Task<IEnumerable<SessionMessage>> GetRecentBySessionIdAsync(Guid sessionId, int count = 50)
        {
            return await _context.SessionMessages
                .Include(m => m.Sender)
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<SessionMessage> AddAsync(SessionMessage message)
        {
            await _context.SessionMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<SessionMessage> AddUserMessageAsync(Guid sessionId, Guid userId, string content)
        {
            var message = new SessionMessage
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                SenderId = userId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsAiGenerated = false
            };

            await _context.SessionMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<SessionMessage> AddAiMessageAsync(Guid sessionId, Guid userId, string content)
        {
            var message = new SessionMessage
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                SenderId = userId, // AI mesajları için de bir sistem kullanıcı ID'si kullanılacak
                Content = content,
                SentAt = DateTime.UtcNow,
                IsAiGenerated = true
            };

            await _context.SessionMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task DeleteAsync(Guid id)
        {
            var message = await _context.SessionMessages.FindAsync(id);
            if (message != null)
            {
                _context.SessionMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetMessageCountAsync(Guid sessionId)
        {
            return await _context.SessionMessages
                .Where(m => m.SessionId == sessionId)
                .CountAsync();
        }

        public async Task<SessionMessage> GetLastMessageAsync(Guid sessionId)
        {
            return await _context.SessionMessages
                .Where(m => m.SessionId == sessionId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }
    }
}