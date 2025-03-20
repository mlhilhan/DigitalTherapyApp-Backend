namespace DigitalTherapyBackendApp.Api.Features.Auth.Payloads
{
    public class LoginPayload
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
