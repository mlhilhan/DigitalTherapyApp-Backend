using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class TherapySessionConfiguration : IEntityTypeConfiguration<TherapySession>
    {
        public void Configure(EntityTypeBuilder<TherapySession> builder)
        {
            // Tablo adı
            builder.ToTable("TherapySessions");

            // Primary key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.PatientId).IsRequired();
            builder.Property(t => t.IsAiSession).IsRequired().HasDefaultValue(false);
            builder.Property(t => t.StartTime).IsRequired();
            builder.Property(t => t.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Scheduled");
            builder.Property(t => t.Summary).HasMaxLength(2000);
            builder.Property(t => t.SessionType).HasMaxLength(20).HasDefaultValue("Text");
            builder.Property(t => t.MeetingLink).HasMaxLength(500);

            // İlişkiler
            builder.HasOne(t => t.Patient)
                .WithMany()
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Therapist)
                .WithMany()
                .HasForeignKey(t => t.PsychologistId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Relationship)
                .WithMany()
                .HasForeignKey(t => t.RelationshipId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
