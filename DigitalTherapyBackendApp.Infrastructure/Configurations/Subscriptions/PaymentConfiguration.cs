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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Table name
            builder.ToTable("Payments");

            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.UserSubscriptionId).IsRequired();
            builder.Property(p => p.TransactionId).HasMaxLength(100);
            builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.Currency).IsRequired().HasMaxLength(10);
            builder.Property(p => p.Status).IsRequired().HasMaxLength(20);
            builder.Property(p => p.PaymentMethod).IsRequired().HasMaxLength(50);
            builder.Property(p => p.PaymentDetails).HasMaxLength(1000);
            builder.Property(p => p.PaymentDate).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();

            // Relationships
            builder.HasOne(p => p.UserSubscription)
                .WithMany()
                .HasForeignKey(p => p.UserSubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
