using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class SessionMessageConfiguration : IEntityTypeConfiguration<SessionMessage>
    {
        public void Configure(EntityTypeBuilder<SessionMessage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.SentAt).IsRequired();
            builder.Property(x => x.IsAiGenerated).IsRequired();

            // Session ile ilişki
            builder.HasOne(x => x.Session)
                .WithMany(s => s.Messages)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Gönderen (User) ile ilişki
            builder.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}