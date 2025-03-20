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
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Summary)
                .HasMaxLength(2000);

            builder.Property(s => s.StartTime)
                .IsRequired();

            // Relationships

            // Patient relationship
            builder.HasOne(s => s.Patient)
                .WithMany()
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Therapist relationship (optional)
            builder.HasOne(s => s.Therapist)
                .WithMany()
                .HasForeignKey(s => s.TherapistId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Messages relationship
            builder.HasMany(s => s.Messages)
                .WithOne(m => m.Session)
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
