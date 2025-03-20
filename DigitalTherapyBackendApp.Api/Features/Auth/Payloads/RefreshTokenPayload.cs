namespace DigitalTherapyBackendApp.Api.Features.Auth.Payloads
{
    public class RefreshTokenPayload
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
