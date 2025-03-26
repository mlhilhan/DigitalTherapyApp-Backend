using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class EmotionalStateConfiguration : IEntityTypeConfiguration<EmotionalState>
    {
        public void Configure(EntityTypeBuilder<EmotionalState> builder)
        {
            builder.ToTable("EmotionalStates");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.UserId).IsRequired().HasColumnType("uuid");

            builder.Property(e => e.MoodLevel).IsRequired();

            // string listesini saklama - jsonb formatında
            builder.Property(e => e.Factors)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions()) : null,
                    v => v != null ? JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) : new List<string>());

            builder.Property(e => e.Notes).HasColumnType("text");
            builder.Property(e => e.Date).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");
            builder.Property(e => e.IsBookmarked).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            // Indexler
            builder.HasIndex(e => e.UserId).HasDatabaseName("IX_EmotionalStates_UserId");
            builder.HasIndex(e => e.Date).HasDatabaseName("IX_EmotionalStates_Date");
            builder.HasIndex(e => e.IsBookmarked).HasDatabaseName("IX_EmotionalStates_IsBookmarked");
            builder.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_EmotionalStates_IsDeleted");

            // Composite index - kullanıcı bazlı tarih sıralaması için
            builder.HasIndex(e => new { e.UserId, e.Date }).HasDatabaseName("IX_EmotionalStates_UserId_Date");
        }
    }
}