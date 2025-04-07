using DigitalTherapyBackendApp.Domain.Entities.DailyTip;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class DailyTipConfiguration : IEntityTypeConfiguration<DailyTip>
    {
        public void Configure(EntityTypeBuilder<DailyTip> builder)
        {
            builder.ToTable("DailyTips");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.TipKey).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Icon).HasMaxLength(50);
            builder.Property(e => e.Color).HasMaxLength(20);
            builder.Property(e => e.IsFeatured).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.IsBookmarked).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            builder.HasIndex(e => e.TipKey).IsUnique();

            builder.HasOne(e => e.Category)
                .WithMany(c => c.Tips)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}