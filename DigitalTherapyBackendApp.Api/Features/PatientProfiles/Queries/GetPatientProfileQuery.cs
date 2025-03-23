using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.PatientProfiles.Queries
{
    public class GetPatientProfileQuery : IRequest<GetPatientProfileResponse>
    {
        public Guid UserId { get; }

        public GetPatientProfileQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetPatientProfileQueryHandler : IRequestHandler<GetPatientProfileQuery, GetPatientProfileResponse>
    {
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly ILogger<GetPatientProfileQueryHandler> _logger;
        private readonly string _baseUrl;

        public GetPatientProfileQueryHandler(
            IPatientProfileRepository patientProfileRepository,
            ILogger<GetPatientProfileQueryHandler> logger,
            IConfiguration configuration)
        {
            _patientProfileRepository = patientProfileRepository;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task<GetPatientProfileResponse> Handle(GetPatientProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _patientProfileRepository.GetByUserIdAsync(request.UserId);
                if (profile == null)
                {
                    _logger.LogWarning("Patient profile not found for user: {UserId}", request.UserId);
                    return new GetPatientProfileResponse
                    {
                        Success = false,
                        Message = "Patient profile not found."
                    };
                }

                var avatarUrl = !string.IsNullOrEmpty(profile.AvatarUrl)
                    ? $"{_baseUrl}{profile.AvatarUrl}"
                    : null;

                return new GetPatientProfileResponse
                {
                    Success = true,
                    Data = new PatientProfileDto
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
                        PhoneNumberConfirmed = profile.User?.PhoneNumberConfirmed
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient profile for user: {UserId}", request.UserId);
                return new GetPatientProfileResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the patient profile."
                };
            }
        }
    }
}
