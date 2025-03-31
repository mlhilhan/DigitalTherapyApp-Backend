using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface IAiService
    {
        /// <summary>
        /// Verilen mesaja yapay zeka yanıtı oluşturur ve kaydeder
        /// </summary>
        /// <param name="patientId">Hastanın ID'si</param>
        /// <param name="message">Hastanın mesajı</param>
        /// <returns>Yapay zeka yanıtı</returns>
        Task<string> GetChatResponseAsync(Guid patientId, string message);

        /// <summary>
        /// AI sohbet oturumunu sonlandırır
        /// </summary>
        /// <param name="sessionId">Sonlandırılacak oturum ID'si</param>
        /// <returns>İşlem başarılı ise true, değilse false</returns>
        Task<bool> EndChatSessionAsync(Guid sessionId);
    }
}
