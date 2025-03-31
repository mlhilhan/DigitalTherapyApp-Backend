using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class SessionMessageRepository : ISessionMessageRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SessionMessageRepository> _logger;

        public SessionMessageRepository(
            AppDbContext context,
            ILogger<SessionMessageRepository> logger)
        {
            _context = context;
            _logger = logger;
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
                .OrderByDescending(m => m.SentAt)
                .Take(count)
                .OrderBy(m => m.SentAt)
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
            try
            {
                // Önce kullanıcının varlığını kontrol et
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    _logger.LogWarning($"User with ID {userId} does not exist. Cannot add user message.");
                    throw new InvalidOperationException($"User with ID {userId} does not exist.");
                }

                // Session'ın varlığını kontrol et
                var sessionExists = await _context.TherapySessions.AnyAsync(s => s.Id == sessionId);
                if (!sessionExists)
                {
                    _logger.LogWarning($"Session with ID {sessionId} does not exist. Cannot add user message.");
                    throw new InvalidOperationException($"Session with ID {sessionId} does not exist.");
                }

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
                _logger.LogInformation($"User message added: SessionID={sessionId}, UserID={userId}");
                return message;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error adding user message: SessionID={sessionId}, UserID={userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding user message: SessionID={sessionId}, UserID={userId}");
                throw;
            }
        }

        public async Task<SessionMessage> AddAiMessageAsync(Guid sessionId, Guid userId, string content)
        {
            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    _logger.LogWarning($"User with ID {userId} does not exist. Cannot add AI message.");
                    throw new InvalidOperationException($"User with ID {userId} does not exist.");
                }

                var sessionExists = await _context.TherapySessions.AnyAsync(s => s.Id == sessionId);
                if (!sessionExists)
                {
                    _logger.LogWarning($"Session with ID {sessionId} does not exist. Cannot add AI message.");
                    throw new InvalidOperationException($"Session with ID {sessionId} does not exist.");
                }

                var message = new SessionMessage
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    SenderId = userId,
                    Content = content,
                    SentAt = DateTime.UtcNow,
                    IsAiGenerated = true
                };

                await _context.SessionMessages.AddAsync(message);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"AI message added: SessionID={sessionId}, UserID={userId}");
                return message;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error adding AI message: SessionID={sessionId}, UserID={userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding AI message: SessionID={sessionId}, UserID={userId}");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var message = await _context.SessionMessages.FindAsync(id);
            if (message != null)
            {
                _context.SessionMessages.Remove(message);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Message deleted: ID={id}");
            }
            else
            {
                _logger.LogWarning($"Attempted to delete non-existent message: ID={id}");
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