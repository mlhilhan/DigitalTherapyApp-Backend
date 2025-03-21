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
    public class TherapistPatientRelationshipConfiguration : IEntityTypeConfiguration<TherapistPatientRelationship>
    {
        public void Configure(EntityTypeBuilder<TherapistPatientRelationship> builder)
        {
            // Tablo adı
            builder.ToTable("TherapistPatientRelationships");

            // Primary key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.StartDate).IsRequired();
            builder.Property(t => t.Status).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Notes).HasMaxLength(1000);
            builder.Property(t => t.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

            // İlişkiler
            builder.HasOne(t => t.Psychologist)
                .WithMany()
                .HasForeignKey(t => t.PsychologistId)
                .OnDelete(DeleteBehavior.Restrict); // Psikolog silindiğinde ilişki silinmez

            builder.HasOne(t => t.Patient)
                .WithMany()
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // Hasta silindiğinde ilişki silinmez

            // İndexler
            builder.HasIndex(t => new { t.PsychologistId, t.PatientId, t.Status })
                .HasDatabaseName("IX_TherapistPatientRelationship_PsychologistPatientStatus");
        }
    }
}
