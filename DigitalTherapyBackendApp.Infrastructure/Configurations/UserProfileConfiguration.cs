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
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .HasMaxLength(100);

            builder.Property(p => p.Gender)
                .HasMaxLength(50);

            builder.Property(p => p.Bio)
                .HasMaxLength(1000);

            builder.Property(p => p.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(p => p.PreferredLanguage)
                .HasMaxLength(50);

            builder.Property(p => p.NotificationPreferences)
                .HasMaxLength(2000);

            // Relationship - one-to-one with User
            builder.HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
