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
    public class SessionMessageConfiguration : IEntityTypeConfiguration<SessionMessage>
    {
        public void Configure(EntityTypeBuilder<SessionMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(m => m.SentAt)
                .IsRequired();

            builder.Property(m => m.IsAiGenerated)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships

            // Session relationship
            builder.HasOne(m => m.Session)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sender relationship
            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
