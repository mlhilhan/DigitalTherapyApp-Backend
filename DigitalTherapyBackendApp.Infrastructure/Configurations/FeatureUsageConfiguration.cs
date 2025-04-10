using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class FeatureUsageConfiguration : IEntityTypeConfiguration<FeatureUsage>
    {
        public void Configure(EntityTypeBuilder<FeatureUsage> builder)
        {
            builder.ToTable("FeatureUsages");

            builder.HasKey(fu => fu.Id);

            builder.Property(fu => fu.Id).ValueGeneratedOnAdd();
            builder.Property(fu => fu.UserId).IsRequired();
            builder.Property(fu => fu.FeatureName).IsRequired().HasMaxLength(50);
            builder.Property(fu => fu.UsageTime).IsRequired();

            builder.HasOne(fu => fu.User)
                   .WithMany()
                   .HasForeignKey(fu => fu.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(fu => new { fu.UserId, fu.FeatureName, fu.UsageTime });
        }
    }
}