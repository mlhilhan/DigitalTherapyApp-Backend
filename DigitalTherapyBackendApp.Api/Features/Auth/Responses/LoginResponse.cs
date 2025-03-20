namespace DigitalTherapyBackendApp.Api.Features.Auth.Responses
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ErrorCode { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserData User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
