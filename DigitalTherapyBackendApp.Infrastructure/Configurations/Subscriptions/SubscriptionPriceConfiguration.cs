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
    public class SubscriptionPriceConfiguration : IEntityTypeConfiguration<SubscriptionPrice>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPrice> builder)
        {
            builder.ToTable("SubscriptionPrices");
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.CountryCode).IsRequired().HasMaxLength(2);
            builder.Property(sp => sp.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.Property(sp => sp.Price).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(sp => sp.Subscription)
                   .WithMany(s => s.PricesByCountry)
                   .HasForeignKey(sp => sp.SubscriptionId);
        }
    }
}
