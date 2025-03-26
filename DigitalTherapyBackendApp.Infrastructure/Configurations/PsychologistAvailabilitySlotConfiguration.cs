using DigitalTherapyBackendApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalTherapyBackendApp.Infrastructure.Configurations
{
    public class PsychologistAvailabilitySlotConfiguration : IEntityTypeConfiguration<PsychologistAvailabilitySlot>
    {
        public void Configure(EntityTypeBuilder<PsychologistAvailabilitySlot> builder)
        {
            // Table name
            builder.ToTable("PsychologistAvailabilitySlots");

            // Primary key
            builder.HasKey(a => a.Id);

            // Properties
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.PsychologistProfileId).IsRequired();
            builder.Property(a => a.DayOfWeek).IsRequired();
            builder.Property(a => a.StartTime).IsRequired();
            builder.Property(a => a.EndTime).IsRequired();

            // Relationships
            builder.HasOne(a => a.PsychologistProfile)
                .WithMany(p => p.AvailabilitySlots)
                .HasForeignKey(a => a.PsychologistProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}