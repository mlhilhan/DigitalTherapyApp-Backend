using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Application.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Dosyayı belirtilen klasöre kaydeder
        /// </summary>
        /// <param name="fileStream">Dosya içeriği</param>
        /// <param name="fileName">Dosya adı (uzantısız)</param>
        /// <param name="fileExtension">Dosya uzantısı (.jpg, .png vb.)</param>
        /// <param name="folderName">Alt klasör adı (avatars, documents vb.)</param>
        /// <returns>Kaydedilen dosyanın tam yolu veya URL'i</returns>
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string fileExtension, string folderName = null);

        /// <summary>
        /// Belirtilen yoldaki dosyayı siler
        /// </summary>
        /// <param name="filePath">Silinecek dosyanın yolu veya URL'i</param>
        /// <returns>İşlemin başarılı olup olmadığı</returns>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Verilen dosya yolundan bir dosyayı okur
        /// </summary>
        /// <param name="filePath">Dosya yolu veya URL</param>
        /// <returns>Dosya içeriği</returns>
        Task<Stream> GetFileAsync(string filePath);
    }
}
