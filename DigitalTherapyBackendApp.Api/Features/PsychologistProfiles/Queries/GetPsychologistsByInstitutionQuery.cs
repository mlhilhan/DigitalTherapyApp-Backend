using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Queries
{
    public class GetPsychologistsByInstitutionQuery : IRequest<GetPsychologistListResponse>
    {
        public Guid InstitutionId { get; }

        public GetPsychologistsByInstitutionQuery(Guid institutionId)
        {
            InstitutionId = institutionId;
        }
    }

    public class GetPsychologistsByInstitutionQueryHandler : IRequestHandler<GetPsychologistsByInstitutionQuery, GetPsychologistListResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly ILogger<GetPsychologistsByInstitutionQueryHandler> _logger;
        private readonly string _baseUrl;

        public GetPsychologistsByInstitutionQueryHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            ILogger<GetPsychologistsByInstitutionQueryHandler> logger,
            IConfiguration configuration)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task<GetPsychologistListResponse> Handle(GetPsychologistsByInstitutionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var psychologists = await _psychologistProfileRepository.GetByInstitutionIdAsync(request.InstitutionId);
                if (psychologists == null || !psychologists.Any())
                {
                    _logger.LogWarning("No psychologists found for institution: {InstitutionId}", request.InstitutionId);
                    return new GetPsychologistListResponse
                    {
                        Success = true,
                        Data = new List<PsychologistProfileDto>(),
                        Message = "No psychologists found for the specified institution."
                    };
                }

                var psychologistDtos = psychologists.Select(profile => new PsychologistProfileDto
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    AvatarUrl = !string.IsNullOrEmpty(profile.AvatarUrl) ? $"{_baseUrl}{profile.AvatarUrl}" : null,
                    Bio = profile.Bio,
                    InstitutionId = profile.InstitutionId,
                    InstitutionName = profile.Institution?.Name,
                    Education = profile.Education,
                    Experience = profile.Experience,
                    Specialties = profile.Specialties?.Select(s => s.Name).ToList() ?? new List<string>()
                }).ToList();

                return new GetPsychologistListResponse
                {
                    Success = true,
                    Data = psychologistDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving psychologists for institution: {InstitutionId}", request.InstitutionId);
                return new GetPsychologistListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the psychologists list."
                };
            }
        }
    }
}