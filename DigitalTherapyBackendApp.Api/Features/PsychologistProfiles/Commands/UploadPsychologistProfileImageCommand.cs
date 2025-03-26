using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Commands
{
    public class UploadPsychologistProfileImageCommand : IRequest<UploadPsychologistProfileImageResponse>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        public IFormFile Image { get; set; }
    }

    public class UploadPsychologistProfileImageCommandHandler : IRequestHandler<UploadPsychologistProfileImageCommand, UploadPsychologistProfileImageResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UploadPsychologistProfileImageCommandHandler> _logger;
        private readonly string _baseUrl;

        public UploadPsychologistProfileImageCommandHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            IFileStorageService fileStorageService,
            ILogger<UploadPsychologistProfileImageCommandHandler> logger,
            IConfiguration configuration)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task<UploadPsychologistProfileImageResponse> Handle(UploadPsychologistProfileImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _psychologistProfileRepository.GetByUserIdAsync(request.UserId);
                if (profile == null)
                {
                    _logger.LogWarning("Psychologist profile not found for user: {UserId}", request.UserId);
                    return new UploadPsychologistProfileImageResponse
                    {
                        Success = false,
                        Message = "No psychologist profile found."
                    };
                }

                if (request.Image == null || request.Image.Length == 0)
                {
                    return new UploadPsychologistProfileImageResponse
                    {
                        Success = false,
                        Message = "No image found to upload."
                    };
                }

                // Eski resmi sil (eğer varsa)
                if (!string.IsNullOrEmpty(profile.AvatarUrl))
                {
                    await _fileStorageService.DeleteFileAsync(profile.AvatarUrl);
                }

                // Yeni resmi kaydet
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(request.Image.FileName);

                using (var stream = request.Image.OpenReadStream())
                {
                    string filePath = await _fileStorageService.SaveFileAsync(
                        stream,
                        fileName,
                        extension,
                        "psychologists"
                    );

                    // Profil resmini güncelle
                    profile.AvatarUrl = filePath;
                    await _psychologistProfileRepository.UpdateAsync(profile);

                    return new UploadPsychologistProfileImageResponse
                    {
                        Success = true,
                        Data = new UploadPsychologistProfileImageResponse.AvatarData
                        {
                            AvatarUrl = filePath
                        },
                        Message = "Profile picture uploaded successfully."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image for user: {UserId}", request.UserId);
                return new UploadPsychologistProfileImageResponse
                {
                    Success = false,
                    Message = "An error occurred while loading the profile picture: " + ex.Message
                };
            }
        }
    }
}