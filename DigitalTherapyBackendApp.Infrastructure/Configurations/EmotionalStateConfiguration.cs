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
    public class EmotionalStateConfiguration : IEntityTypeConfiguration<EmotionalState>
    {
        public void Configure(EntityTypeBuilder<EmotionalState> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Mood)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.MoodIntensity)
                .IsRequired();

            builder.Property(e => e.Notes)
                .HasMaxLength(1000);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            // Relationship
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
