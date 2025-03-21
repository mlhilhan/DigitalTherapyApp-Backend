using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IDirectMessageRepository
    {
        Task<IEnumerable<DirectMessage>> GetAllAsync();
        Task<DirectMessage> GetByIdAsync(Guid id);
        Task<IEnumerable<DirectMessage>> GetConversationAsync(Guid userId1, Guid userId2, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<DirectMessage>> GetMessagesBySenderAsync(Guid senderId);
        Task<IEnumerable<DirectMessage>> GetMessagesByReceiverAsync(Guid receiverId);
        Task<IEnumerable<DirectMessage>> GetMessagesByRelationshipAsync(Guid relationshipId);
        Task<IEnumerable<DirectMessage>> GetUnreadMessagesAsync(Guid receiverId);
        Task<int> GetUnreadMessageCountAsync(Guid receiverId);
        Task<DirectMessage> AddAsync(DirectMessage message);
        Task<bool> MarkAsReadAsync(Guid messageId);
        Task<bool> MarkConversationAsReadAsync(Guid userId1, Guid userId2);
        Task<bool> DeleteAsync(Guid id);
    }
}
