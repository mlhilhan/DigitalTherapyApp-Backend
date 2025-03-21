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
    public class DirectMessageConfiguration : IEntityTypeConfiguration<DirectMessage>
    {
        public void Configure(EntityTypeBuilder<DirectMessage> builder)
        {
            // Tablo adı
            builder.ToTable("DirectMessages");

            // Primary key
            builder.HasKey(m => m.Id);

            // Properties
            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(m => m.SenderId).IsRequired();
            builder.Property(m => m.ReceiverId).IsRequired();
            builder.Property(m => m.Content).IsRequired().HasMaxLength(4000);
            builder.Property(m => m.SentAt).IsRequired();
            builder.Property(m => m.MessageType).HasMaxLength(20).HasDefaultValue("Text");
            builder.Property(m => m.Attachment).HasMaxLength(500);

            // İlişkiler
            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Relationship)
                .WithMany()
                .HasForeignKey(m => m.RelationshipId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // İndeksler
            builder.HasIndex(m => new { m.SenderId, m.ReceiverId })
                .HasDatabaseName("IX_DirectMessage_Sender_Receiver");

            builder.HasIndex(m => m.SentAt)
                .HasDatabaseName("IX_DirectMessage_SentAt");
        }
    }
}
