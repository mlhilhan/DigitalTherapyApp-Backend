namespace DigitalTherapyBackendApp.Api.Features.UserProfiles.Payloads
{
    public class CreateUserProfilePayload
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string PreferredLanguage { get; set; }
        public string NotificationPreferences { get; set; }
    }
}
