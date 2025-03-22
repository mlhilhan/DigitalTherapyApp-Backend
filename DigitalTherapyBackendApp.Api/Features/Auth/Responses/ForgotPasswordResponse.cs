namespace DigitalTherapyBackendApp.Api.Features.Auth.Responses
{
    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}
