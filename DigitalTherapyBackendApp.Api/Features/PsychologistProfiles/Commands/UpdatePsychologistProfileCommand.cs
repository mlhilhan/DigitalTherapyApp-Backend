using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Payloads;
using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Commands
{
    public class UpdatePsychologistProfileCommand : IRequest<UpdatePsychologistProfileResponse>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? NotificationPreferences { get; set; }
        public Guid? InstitutionId { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Experience { get; set; }
        public string? LicenseNumber { get; set; }

        public static UpdatePsychologistProfileCommand FromPayload(UpdatePsychologistProfilePayload payload, Guid userId)
        {
            return new UpdatePsychologistProfileCommand
            {
                UserId = userId,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                BirthDate = payload.BirthDate,
                Gender = payload.Gender,
                Bio = payload.Bio,
                PreferredLanguage = payload.PreferredLanguage,
                NotificationPreferences = payload.NotificationPreferences,
                InstitutionId = payload.InstitutionId,
                Education = payload.Education,
                Certifications = payload.Certifications,
                Experience = payload.Experience,
                LicenseNumber = payload.LicenseNumber
            };
        }
    }

    public class UpdatePsychologistProfileCommandHandler : IRequestHandler<UpdatePsychologistProfileCommand, UpdatePsychologistProfileResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly ILogger<UpdatePsychologistProfileCommandHandler> _logger;
        private readonly string _baseUrl;

        public UpdatePsychologistProfileCommandHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            ILogger<UpdatePsychologistProfileCommandHandler> logger,
            IConfiguration configuration)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task<UpdatePsychologistProfileResponse> Handle(UpdatePsychologistProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _psychologistProfileRepository.GetByUserIdAsync(request.UserId);
                if (profile == null)
                {
                    _logger.LogWarning("Psychologist profile not found for user: {UserId}", request.UserId);
                    return new UpdatePsychologistProfileResponse
                    {
                        Success = false,
                        Message = "Psychologist profile not found."
                    };
                }

                if (!string.IsNullOrEmpty(request.FirstName))
                    profile.FirstName = request.FirstName;

                if (!string.IsNullOrEmpty(request.LastName))
                    profile.LastName = request.LastName;

                if (request.BirthDate.HasValue)
                    profile.BirthDate = request.BirthDate;

                if (!string.IsNullOrEmpty(request.Gender))
                    profile.Gender = request.Gender;

                if (!string.IsNullOrEmpty(request.Bio))
                    profile.Bio = request.Bio;

                if (!string.IsNullOrEmpty(request.PreferredLanguage))
                    profile.PreferredLanguage = request.PreferredLanguage;

                if (!string.IsNullOrEmpty(request.NotificationPreferences))
                    profile.NotificationPreferences = request.NotificationPreferences;

                if (request.InstitutionId.HasValue)
                    profile.InstitutionId = request.InstitutionId;

                if (!string.IsNullOrEmpty(request.Education))
                    profile.Education = request.Education;

                if (!string.IsNullOrEmpty(request.Certifications))
                    profile.Certifications = request.Certifications;

                if (!string.IsNullOrEmpty(request.Experience))
                    profile.Experience = request.Experience;

                if (!string.IsNullOrEmpty(request.LicenseNumber))
                    profile.LicenseNumber = request.LicenseNumber;

                await _psychologistProfileRepository.UpdateAsync(profile);

                var avatarUrl = !string.IsNullOrEmpty(profile.AvatarUrl)
                    ? $"{_baseUrl}{profile.AvatarUrl}"
                    : null;

                return new UpdatePsychologistProfileResponse
                {
                    Success = true,
                    Data = new PsychologistProfileDto
                    {
                        Id = profile.Id,
                        UserId = profile.UserId,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        BirthDate = profile?.BirthDate,
                        Gender = profile?.Gender,
                        Bio = profile?.Bio,
                        AvatarUrl = avatarUrl,
                        PreferredLanguage = profile?.PreferredLanguage,
                        NotificationPreferences = profile?.NotificationPreferences,
                        Email = profile.User?.Email,
                        EmailConfirmed = profile.User?.EmailConfirmed,
                        PhoneNumber = profile.User?.PhoneNumber,
                        PhoneNumberConfirmed = profile.User?.PhoneNumberConfirmed,
                        InstitutionId = profile.InstitutionId,
                        InstitutionName = profile.Institution?.Name,
                        Education = profile.Education,
                        Certifications = profile.Certifications,
                        Experience = profile.Experience,
                        LicenseNumber = profile.LicenseNumber,
                        Specialties = profile.Specialties?.Select(s => s.Name).ToList() ?? new List<string>()
                    },
                    Message = "Psychologist profile updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating psychologist profile for user: {UserId}", request.UserId);
                return new UpdatePsychologistProfileResponse
                {
                    Success = false,
                    Message = "An error occurred while updating the psychologist profile."
                };
            }
        }
    }
}