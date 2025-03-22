using DigitalTherapyBackendApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IInstitutionRepository
    {
        /// <summary>
        /// Tüm kurumları getirir
        /// </summary>
        Task<IEnumerable<InstitutionProfile>> GetAllAsync();

        /// <summary>
        /// ID'ye göre kurum getirir
        /// </summary>
        Task<InstitutionProfile> GetByIdAsync(Guid id);

        /// <summary>
        /// Kullanıcı ID'sine göre kurum getirir
        /// </summary>
        Task<InstitutionProfile> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Tüm doğrulanmış kurumları getirir
        /// </summary>
        Task<IEnumerable<InstitutionProfile>> GetVerifiedInstitutionsAsync();

        /// <summary>
        /// Kuruma bağlı tüm psikologları getirir
        /// </summary>
        Task<IEnumerable<PsychologistProfile>> GetPsychologistsAsync(Guid institutionId);

        /// <summary>
        /// Yeni kurum ekler
        /// </summary>
        Task<InstitutionProfile> AddAsync(InstitutionProfile institution);

        /// <summary>
        /// Kurumu günceller
        /// </summary>
        Task<InstitutionProfile> UpdateAsync(InstitutionProfile institution);

        /// <summary>
        /// Kurumu siler
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Kurumun doğrulama durumunu günceller
        /// </summary>
        Task<bool> UpdateVerificationStatusAsync(Guid id, bool isVerified);

        /// <summary>
        /// Kullanıcı ID'sine göre kurumun var olup olmadığını kontrol eder
        /// </summary>
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
}
