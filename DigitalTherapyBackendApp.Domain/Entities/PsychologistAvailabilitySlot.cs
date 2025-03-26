using System;

namespace DigitalTherapyBackendApp.Domain.Entities
{
    public class PsychologistAvailabilitySlot
    {
        public Guid Id { get; set; }
        public Guid PsychologistProfileId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public PsychologistProfile PsychologistProfile { get; set; }
    }
}