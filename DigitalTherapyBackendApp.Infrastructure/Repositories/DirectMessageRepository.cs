using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Repositories
{
    public class DirectMessageRepository : IDirectMessageRepository
    {
        private readonly AppDbContext _context;

        public DirectMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DirectMessage>> GetAllAsync()
        {
            return await _context.DirectMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        public async Task<DirectMessage> GetByIdAsync(Guid id)
        {
            return await _context.DirectMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<DirectMessage>> GetConversationAsync(Guid userId1, Guid userId2, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.DirectMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<DirectMessage>> GetMessagesBySenderAsync(Guid senderId)
        {
            return await _context.DirectMessages
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == senderId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DirectMessage>> GetMessagesByReceiverAsync(Guid receiverId)
        {
            return await _context.DirectMessages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == receiverId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DirectMessage>> GetMessagesByRelationshipAsync(Guid relationshipId)
        {
            return await _context.DirectMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.RelationshipId == relationshipId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DirectMessage>> GetUnreadMessagesAsync(Guid receiverId)
        {
            return await _context.DirectMessages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == receiverId && !m.IsRead)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid receiverId)
        {
            return await _context.DirectMessages
                .CountAsync(m => m.ReceiverId == receiverId && !m.IsRead);
        }

        public async Task<DirectMessage> AddAsync(DirectMessage message)
        {
            message.SentAt = DateTime.UtcNow;
            message.IsRead = false;

            await _context.DirectMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> MarkAsReadAsync(Guid messageId)
        {
            var message = await _context.DirectMessages.FindAsync(messageId);
            if (message == null || message.IsRead)
                return false;

            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkConversationAsReadAsync(Guid userId1, Guid userId2)
        {
            var messages = await _context.DirectMessages
                .Where(m =>
                    m.ReceiverId == userId1 &&
                    m.SenderId == userId2 &&
                    !m.IsRead)
                .ToListAsync();

            if (!messages.Any())
                return false;

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var message = await _context.DirectMessages.FindAsync(id);
            if (message == null)
                return false;

            _context.DirectMessages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
