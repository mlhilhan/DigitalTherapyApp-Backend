using DigitalTherapyBackendApp.Domain.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations.Subscriptions
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Domain.Entities.Subscriptions.Subscription>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Subscriptions.Subscription> builder)
        {
            builder.ToTable("Subscriptions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Description).HasMaxLength(500);
            builder.Property(s => s.BasePrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(s => s.BaseCurrency).IsRequired().HasMaxLength(50);
            builder.Property(s => s.PlanId).IsRequired().HasMaxLength(50);
            builder.Property(s => s.DurationInDays).IsRequired();
            builder.Property(s => s.IsActive).IsRequired();
            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.UpdatedAt).IsRequired();
            builder.Property(s => s.MoodEntryLimit).IsRequired();
            builder.Property(s => s.AIChatSessionsPerWeek).IsRequired();
            builder.Property(s => s.MessageLimitPerChat).IsRequired();
            builder.Property(s => s.HasPsychologistSupport).IsRequired();
            builder.Property(s => s.PsychologistSessionsPerMonth).IsRequired();
            builder.Property(s => s.HasEmergencySupport).IsRequired();
            builder.Property(s => s.HasAdvancedReports).IsRequired();
            builder.Property(s => s.HasAllMeditationContent).IsRequired();

            // Relationships
            builder.HasMany(s => s.UserSubscriptions)
                .WithOne(us => us.Subscription)
                .HasForeignKey(us => us.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
