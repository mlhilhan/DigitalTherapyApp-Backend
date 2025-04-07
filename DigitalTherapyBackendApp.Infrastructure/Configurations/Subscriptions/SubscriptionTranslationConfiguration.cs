using DigitalTherapyBackendApp.Domain.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations.Subscriptions
{
    public class SubscriptionTranslationConfiguration : IEntityTypeConfiguration<SubscriptionTranslation>
    {
        public void Configure(EntityTypeBuilder<SubscriptionTranslation> builder)
        {
            // Table name
            builder.ToTable("SubscriptionTranslations");

            // Primary key
            builder.HasKey(st => st.Id);

            // Properties
            builder.Property(st => st.Id).ValueGeneratedOnAdd();
            builder.Property(st => st.SubscriptionId).IsRequired();
            builder.Property(st => st.LanguageCode).IsRequired().HasMaxLength(5);
            builder.Property(st => st.Name).IsRequired().HasMaxLength(100);
            builder.Property(st => st.Description).HasMaxLength(500);

            // Relationships
            builder.HasOne(st => st.Subscription)
                   .WithMany(s => s.Translations)
                   .HasForeignKey(st => st.SubscriptionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(st => new { st.SubscriptionId, st.LanguageCode }).IsUnique();
        }
    }
}