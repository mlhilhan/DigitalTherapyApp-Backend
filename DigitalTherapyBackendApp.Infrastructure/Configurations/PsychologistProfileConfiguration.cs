﻿using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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

            // Yeni alanlar için yapılandırma
            builder.Property(p => p.InstitutionName).HasMaxLength(200);
            builder.Property(p => p.Education).HasMaxLength(1000);
            builder.Property(p => p.Certifications).HasMaxLength(1000);
            builder.Property(p => p.Experience).HasMaxLength(1000);
            builder.Property(p => p.LicenseNumber).HasMaxLength(100);
            builder.Property(p => p.IsAvailable).HasDefaultValue(false);

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

            // Many-to-many relationship with Specialties
            builder.HasMany(p => p.Specialties)
                .WithMany(s => s.Psychologists)
                .UsingEntity(j => j.ToTable("PsychologistSpecialties"));
        }
    }
}