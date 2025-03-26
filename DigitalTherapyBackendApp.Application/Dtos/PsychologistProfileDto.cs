namespace DigitalTherapyBackendApp.Application.Dtos
{
    public class PsychologistProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? NotificationPreferences { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public Guid? InstitutionId { get; set; }
        public string? InstitutionName { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Experience { get; set; }
        public string? LicenseNumber { get; set; }
        public List<string> Specialties { get; set; } = new List<string>();
        public bool IsAvailable { get; set; }
        public List<AvailabilitySlotDto> AvailabilitySlots { get; set; } = new List<AvailabilitySlotDto>();
    }
}