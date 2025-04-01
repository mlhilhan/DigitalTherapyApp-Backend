using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface IAiService
    {
        Task<string> GetChatResponseAsync(Guid patientId, string message, Guid? sessionId = null);
        Task<bool> EndChatSessionAsync(Guid sessionId);
    }
}
