using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;
using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;

namespace DigitalTherapyBackendApp.Api.Features.PatientProfiles.Commands
{
    public class UploadPatientProfileImageCommand : IRequest<GetPatientProfileResponse>
    {
        public Guid UserId { get; set; }
        public IFormFile Image { get; set; }
    }

    public class UploadPatientProfileImageCommandHandler : IRequestHandler<UploadPatientProfileImageCommand, GetPatientProfileResponse>
    {
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UploadPatientProfileImageCommandHandler> _logger;

        public UploadPatientProfileImageCommandHandler(
            IPatientProfileRepository patientProfileRepository,
            IFileStorageService fileStorageService,
            ILogger<UploadPatientProfileImageCommandHandler> logger)
        {
            _patientProfileRepository = patientProfileRepository;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<GetPatientProfileResponse> Handle(UploadPatientProfileImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Image == null || request.Image.Length == 0)
                {
                    return new GetPatientProfileResponse
                    {
                        Success = false,
                        Message = "No image file provided."
                    };
                }

                var profile = await _patientProfileRepository.GetByUserIdAsync(request.UserId);
                if (profile == null)
                {
                    return new GetPatientProfileResponse
                    {
                        Success = false,
                        Message = "Patient profile not found."
                    };
                }

                // Eski dosya varsa sil
                if (!string.IsNullOrEmpty(profile.AvatarUrl))
                {
                    await _fileStorageService.DeleteFileAsync(profile.AvatarUrl);
                }

                // Yeni dosyayı yükle
                var fileName = $"patient_{request.UserId}_{DateTime.UtcNow.Ticks}";
                var fileExtension = Path.GetExtension(request.Image.FileName);
                var filePath = await _fileStorageService.SaveFileAsync(request.Image.OpenReadStream(), fileName, fileExtension, "avatars");

                // Profili güncelle
                profile.AvatarUrl = filePath;
                await _patientProfileRepository.UpdateAsync(profile);

                return new GetPatientProfileResponse
                {
                    Success = true,
                    Message = "Profile image uploaded successfully.",
                    Data = new PatientProfileDto
                    {
                        Id = profile.Id,
                        UserId = profile.UserId,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        BirthDate = profile.BirthDate,
                        Gender = profile.Gender,
                        Bio = profile.Bio,
                        AvatarUrl = profile.AvatarUrl,
                        PreferredLanguage = profile.PreferredLanguage,
                        NotificationPreferences = profile.NotificationPreferences
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image for user: {UserId}", request.UserId);
                return new GetPatientProfileResponse
                {
                    Success = false,
                    Message = "An error occurred while uploading the profile image."
                };
            }
        }
    }
}
