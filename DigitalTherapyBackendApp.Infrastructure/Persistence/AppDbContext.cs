using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DigitalTherapyBackendApp.Infrastructure.Persistence
{
    public class AppDbContext: IdentityDbContext<User, Role, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EmotionalState> EmotionalStates { get; set; }
        public DbSet<TherapySession> TherapySessions { get; set; }
        public DbSet<SessionMessage> SessionMessages { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }


        // Configuration Class
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new EmotionalStateConfiguration());
            modelBuilder.ApplyConfiguration(new TherapySessionConfiguration());
            modelBuilder.ApplyConfiguration(new SessionMessageConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());

            // User - Role One-to-Many ilişkisi
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)  // Kullanıcının bir rolü var
                .WithMany(r => r.Users)  // Bir rol, birçok kullanıcıya sahip olabilir
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Rol silinirse kullanıcı silinmemeli
        }
    }
}
