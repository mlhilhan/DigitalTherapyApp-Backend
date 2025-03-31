using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class TherapySessionConfiguration : IEntityTypeConfiguration<TherapySession>
    {
        public void Configure(EntityTypeBuilder<TherapySession> builder)
        {
            builder.ToTable("TherapySessions");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.IsAiSession).IsRequired();
            builder.Property(x => x.StartTime).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasConversion<string>();
            builder.Property(x => x.Type).IsRequired().HasConversion<string>();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.MeetingLink).HasMaxLength(1000);


            // Hasta ile ilişki (Required)
            builder.HasOne(x => x.Patient)
                .WithMany()
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(x => x.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Psikolog ile ilişki (AI oturumları için null)
            builder.HasOne(x => x.Psychologist)
                .WithMany()
                .HasForeignKey(x => x.PsychologistId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Terapi ilişkisi (AI oturumları için null)
            builder.HasOne(x => x.Relationship)
                .WithMany()
                .HasForeignKey(x => x.RelationshipId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Oturum mesajları ile ilişki
            builder.HasMany(x => x.Messages)
                .WithOne(m => m.Session)
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Duygusal durum kayıtları ile ilişki
            builder.HasMany(x => x.EmotionalStates)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}