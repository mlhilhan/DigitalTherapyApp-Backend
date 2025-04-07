using DigitalTherapyBackendApp.Domain.Entities.DailyTip;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class DailyTipTranslationConfiguration : IEntityTypeConfiguration<DailyTipTranslation>
    {
        public void Configure(EntityTypeBuilder<DailyTipTranslation> builder)
        {
            builder.ToTable("DailyTipTranslations");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.LanguageCode).IsRequired().HasMaxLength(5);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.ShortDescription).HasMaxLength(500);
            builder.Property(e => e.Content).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            builder.HasIndex(e => new { e.TipId, e.LanguageCode }).IsUnique();

            builder.HasOne(e => e.Tip)
                .WithMany(t => t.Translations)
                .HasForeignKey(e => e.TipId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}