using DigitalTherapyBackendApp.Domain.Entities.DailyTip;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class DailyTipCategoryConfiguration : IEntityTypeConfiguration<DailyTipCategory>
    {
        public void Configure(EntityTypeBuilder<DailyTipCategory> builder)
        {
            builder.ToTable("DailyTipCategories");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.CategoryKey).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Icon).HasMaxLength(50);
            builder.Property(e => e.Color).HasMaxLength(20);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            builder.HasIndex(e => e.CategoryKey).IsUnique();
        }
    }
}