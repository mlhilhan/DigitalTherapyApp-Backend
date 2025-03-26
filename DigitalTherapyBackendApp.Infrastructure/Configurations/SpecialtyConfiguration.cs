using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            // Table name
            builder.ToTable("Specialties");

            // Primary key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);

            // Unique constraint on Name to prevent duplicates
            builder.HasIndex(s => s.Name).IsUnique();

            // Many-to-many relationship with PsychologistProfile
            builder.HasMany(s => s.Psychologists)
                .WithMany(p => p.Specialties)
                .UsingEntity(j => j.ToTable("PsychologistSpecialties"));
        }
    }
}