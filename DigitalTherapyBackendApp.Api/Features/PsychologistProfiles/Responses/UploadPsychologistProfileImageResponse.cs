
namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses
{
    public class UploadPsychologistProfileImageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AvatarData? Data { get; set; }

        public class AvatarData
        {
            public string? AvatarUrl { get; set; }
        }
    }
}