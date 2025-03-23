namespace DigitalTherapyBackendApp.Api.Features.UserProfiles.Responses
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        public DateOnly? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? NotificationPreferences { get; set; }
    }
}
