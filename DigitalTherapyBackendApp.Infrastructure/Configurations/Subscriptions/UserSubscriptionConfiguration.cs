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
    public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
    {
        public void Configure(EntityTypeBuilder<UserSubscription> builder)
        {
            // Table name
            builder.ToTable("UserSubscriptions");

            // Primary key
            builder.HasKey(us => us.Id);

            // Properties
            builder.Property(us => us.Id).ValueGeneratedOnAdd();
            builder.Property(us => us.UserId).IsRequired();
            builder.Property(us => us.SubscriptionId).IsRequired();
            builder.Property(us => us.StartDate).IsRequired();
            builder.Property(us => us.EndDate).IsRequired();
            builder.Property(us => us.IsActive).IsRequired();
            builder.Property(us => us.AutoRenew).IsRequired();
            builder.Property(us => us.TransactionId).HasMaxLength(100);
            builder.Property(us => us.PaymentMethod).HasMaxLength(50);
            builder.Property(us => us.PaidAmount).HasColumnType("decimal(18,2)");
            builder.Property(us => us.CreatedAt).IsRequired();
            builder.Property(us => us.UpdatedAt).IsRequired();

            // Relationships
            builder.HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(us => us.Subscription)
                .WithMany(s => s.UserSubscriptions)
                .HasForeignKey(us => us.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
