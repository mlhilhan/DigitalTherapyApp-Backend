using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Queries
{
    public class GetPsychologistsBySpecialtyQuery : IRequest<GetPsychologistListResponse>
    {
        public string Specialty { get; }

        public GetPsychologistsBySpecialtyQuery(string specialty)
        {
            Specialty = specialty;
        }
    }

    public class GetPsychologistsBySpecialtyQueryHandler : IRequestHandler<GetPsychologistsBySpecialtyQuery, GetPsychologistListResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly ILogger<GetPsychologistsBySpecialtyQueryHandler> _logger;
        private readonly string _baseUrl;

        public GetPsychologistsBySpecialtyQueryHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            ILogger<GetPsychologistsBySpecialtyQueryHandler> logger,
            IConfiguration configuration)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task<GetPsychologistListResponse> Handle(GetPsychologistsBySpecialtyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var psychologists = await _psychologistProfileRepository.GetBySpecialtyAsync(request.Specialty);
                if (psychologists == null || !psychologists.Any())
                {
                    _logger.LogWarning("No psychologists found with specialty: {Specialty}", request.Specialty);
                    return new GetPsychologistListResponse
                    {
                        Success = true,
                        Data = new List<PsychologistProfileDto>(),
                        Message = $"No psychologists found with the specialty: {request.Specialty}."
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
                _logger.LogError(ex, "Error retrieving psychologists for specialty: {Specialty}", request.Specialty);
                return new GetPsychologistListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the psychologists list."
                };
            }
        }
    }
}