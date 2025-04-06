using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class DailyTipCategoryTranslationConfiguration : IEntityTypeConfiguration<DailyTipCategoryTranslation>
    {
        public void Configure(EntityTypeBuilder<DailyTipCategoryTranslation> builder)
        {
            builder.ToTable("DailyTipCategoryTranslations");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.LanguageCode).IsRequired().HasMaxLength(5);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            builder.HasIndex(e => new { e.CategoryId, e.LanguageCode }).IsUnique();

            builder.HasOne(e => e.Category)
                .WithMany(c => c.Translations)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}