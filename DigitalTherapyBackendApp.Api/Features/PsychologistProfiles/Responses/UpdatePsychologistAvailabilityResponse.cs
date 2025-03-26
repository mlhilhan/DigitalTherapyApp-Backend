using System;
using System.Collections.Generic;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses
{
    public class UpdatePsychologistAvailabilityResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AvailabilityData Data { get; set; }

        public class AvailabilityData
        {
            public bool IsAvailable { get; set; }
            public List<AvailabilitySlotDto> Slots { get; set; }
        }

        public class AvailabilitySlotDto
        {
            public Guid Id { get; set; }
            public DayOfWeek DayOfWeek { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
        }
    }
}