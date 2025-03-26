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
    public class InstitutionProfileConfiguration : IEntityTypeConfiguration<InstitutionProfile>
    {
        public void Configure(EntityTypeBuilder<InstitutionProfile> builder)
        {
            // Table name
            builder.ToTable("InstitutionProfiles");

            // Primary key
            builder.HasKey(i => i.Id);

            // Properties
            builder.Property(i => i.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.UserId).IsRequired();
            builder.Property(i => i.Name).HasMaxLength(200);
            builder.Property(i => i.Description).HasMaxLength(2000);
            builder.Property(i => i.Address).HasMaxLength(500);
            builder.Property(i => i.City).HasMaxLength(100);
            builder.Property(i => i.Phone).HasMaxLength(20);
            builder.Property(i => i.Email).HasMaxLength(255);
            builder.Property(i => i.Website).HasMaxLength(255);
            builder.Property(i => i.PreferredLanguage).HasMaxLength(50);
            builder.Property(i => i.IsVerified).HasDefaultValue(false);

            // Relationships
            builder.HasOne(i => i.User)
                .WithOne()
                .HasForeignKey<InstitutionProfile>(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Institution has many Psychologists
            builder.HasMany(i => i.Psychologists)
                .WithOne(p => p.Institution)
                .HasForeignKey(p => p.InstitutionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
