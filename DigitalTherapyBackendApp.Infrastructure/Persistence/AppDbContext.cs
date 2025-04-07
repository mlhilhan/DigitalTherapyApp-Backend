using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Entities.DailyTip;
using DigitalTherapyBackendApp.Domain.Entities.Subscriptions;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Configurations;
using DigitalTherapyBackendApp.Infrastructure.Configurations.Subscriptions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalTherapyBackendApp.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid>, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EmotionalState> EmotionalStates { get; set; }
        public DbSet<TherapySession> TherapySessions { get; set; }
        public DbSet<SessionMessage> SessionMessages { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<PsychologistProfile> PsychologistProfiles { get; set; }
        public DbSet<InstitutionProfile> InstitutionProfiles { get; set; }
        public DbSet<DirectMessage> DirectMessages { get; set; }
        public DbSet<TherapistPatientRelationship> TherapistPatientRelationships { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<PsychologistAvailabilitySlot> PsychologistAvailabilitySlots { get; set; }
        public DbSet<DailyTipCategory> DailyTipCategories { get; set; }
        public DbSet<DailyTipCategoryTranslation> DailyTipCategoryTranslations { get; set; }
        public DbSet<DailyTip> DailyTips { get; set; }
        public DbSet<DailyTipTranslation> DailyTipTranslations { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SubscriptionPrice> SubscriptionPrices { get; set; }
        public DbSet<SubscriptionTranslation> SubscriptionTranslations { get; set; }


        // Configuration Class
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new EmotionalStateConfiguration());
            modelBuilder.ApplyConfiguration(new TherapySessionConfiguration());
            modelBuilder.ApplyConfiguration(new SessionMessageConfiguration());
            modelBuilder.ApplyConfiguration(new PatientProfileConfiguration());
            modelBuilder.ApplyConfiguration(new PsychologistProfileConfiguration());
            modelBuilder.ApplyConfiguration(new InstitutionProfileConfiguration());
            modelBuilder.ApplyConfiguration(new DirectMessageConfiguration());
            modelBuilder.ApplyConfiguration(new TherapistPatientRelationshipConfiguration());
            modelBuilder.ApplyConfiguration(new SpecialtyConfiguration());
            modelBuilder.ApplyConfiguration(new DailyTipCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new DailyTipCategoryTranslationConfiguration());
            modelBuilder.ApplyConfiguration(new DailyTipConfiguration());
            modelBuilder.ApplyConfiguration(new DailyTipTranslationConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new UserSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionPriceConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionTranslationConfiguration());

            // User - Role One-to-Many ilişkisi
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)  // Kullanıcının bir rolü var
                .WithMany(r => r.Users)  // Bir rol, birçok kullanıcıya sahip olabilir
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Rol silinirse kullanıcı silinmemeli

            modelBuilder.Entity<PsychologistProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PsychologistProfile>(p => p.UserId);

            modelBuilder.Entity<PsychologistProfile>()
                .HasOne(p => p.Institution)
                .WithMany(i => i.Psychologists)
                .HasForeignKey(p => p.InstitutionId)
                .IsRequired(false);
        }

        public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return base.Entry(entity);
        }
    }
}