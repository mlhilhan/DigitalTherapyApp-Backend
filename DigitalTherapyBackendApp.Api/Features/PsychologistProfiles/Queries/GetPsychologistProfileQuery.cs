using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Queries
{
    public class GetPsychologistProfileQuery : IRequest<GetPsychologistProfileResponse>
    {
        public Guid UserId { get; }

        public GetPsychologistProfileQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetPsychologistProfileQueryHandler : IRequestHandler<GetPsychologistProfileQuery, GetPsychologistProfileResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly ILogger<GetPsychologistProfileQueryHandler> _logger;
        private readonly string _baseUrl;

        public GetPsychologistProfileQueryHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            ILogger<GetPsychologistProfileQueryHandler> logger,
            IConfiguration configuration)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task<GetPsychologistProfileResponse> Handle(GetPsychologistProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _psychologistProfileRepository.GetByUserIdAsync(request.UserId);
                if (profile == null)
                {
                    _logger.LogWarning("Psychologist profile not found for user: {UserId}", request.UserId);
                    return new GetPsychologistProfileResponse
                    {
                        Success = false,
                        Message = "Psychologist profile not found."
                    };
                }

                var avatarUrl = !string.IsNullOrEmpty(profile.AvatarUrl)
                    ? $"{_baseUrl}{profile.AvatarUrl}"
                    : null;

                return new GetPsychologistProfileResponse
                {
                    Success = true,
                    Data = new PsychologistProfileDto
                    {
                        Id = profile.Id,
                        UserId = profile.UserId,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        BirthDate = profile.BirthDate,
                        Gender = profile.Gender,
                        Bio = profile.Bio,
                        AvatarUrl = avatarUrl,
                        PreferredLanguage = profile.PreferredLanguage,
                        NotificationPreferences = profile.NotificationPreferences,
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
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving psychologist profile for user: {UserId}", request.UserId);
                return new GetPsychologistProfileResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the psychologist profile."
                };
            }
        }
    }
}