using System.ComponentModel.DataAnnotations;

namespace DigitalTherapyBackendApp.Api.Features.Auth.Payloads
{
    public class ForgotPasswordPayload
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
