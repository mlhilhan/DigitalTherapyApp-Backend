using System.ComponentModel.DataAnnotations;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Payloads
{
    public class UpdatePsychologistAvailabilityPayload
    {
        [Required]
        public bool IsAvailable { get; set; }

        public List<AvailabilitySlotPayload> AvailabilitySlots { get; set; }
    }

    public class AvailabilitySlotPayload
    {
        [Required]
        [Range(0, 6)]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
