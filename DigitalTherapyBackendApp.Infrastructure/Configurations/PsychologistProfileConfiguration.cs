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
    public class PsychologistProfileConfiguration : IEntityTypeConfiguration<PsychologistProfile>
    {
        public void Configure(EntityTypeBuilder<PsychologistProfile> builder)
        {
            // Table name
            builder.ToTable("PsychologistProfiles");

            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.FirstName).HasMaxLength(100);
            builder.Property(p => p.LastName).HasMaxLength(100);
            builder.Property(p => p.Gender).HasMaxLength(20);
            builder.Property(p => p.Bio).HasMaxLength(1000);
            builder.Property(p => p.AvatarUrl).HasMaxLength(500);
            builder.Property(p => p.PreferredLanguage).HasMaxLength(50);
            builder.Property(p => p.NotificationPreferences).HasMaxLength(255);

            // Relationships
            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PsychologistProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Institution - A psychologist can belong to only one institution or be independent
            builder.HasOne(p => p.Institution)
                .WithMany(i => i.Psychologists)
                .HasForeignKey(p => p.InstitutionId)
                .IsRequired(false) // InstitutionId can be null (independent psychologist)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
