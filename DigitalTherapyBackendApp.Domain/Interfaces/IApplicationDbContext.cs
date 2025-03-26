using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<PatientProfile> PatientProfiles { get; set; }
        DbSet<PsychologistProfile> PsychologistProfiles { get; set; }
        DbSet<InstitutionProfile> InstitutionProfiles { get; set; }
        DbSet<Specialty> Specialties { get; set; }
        DbSet<PsychologistAvailabilitySlot> PsychologistAvailabilitySlots { get; set; }
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}