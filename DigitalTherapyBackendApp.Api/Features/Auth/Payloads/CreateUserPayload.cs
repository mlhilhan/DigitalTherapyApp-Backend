namespace DigitalTherapyBackendApp.Api.Features.Auth.Payloads
{
    public class CreateUserPayload
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid RoleId { get; set; }
        public string? PreferredLanguage { get; set; }
    }
}
