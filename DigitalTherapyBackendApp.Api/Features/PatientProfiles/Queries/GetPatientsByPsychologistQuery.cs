using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Responses;
using DigitalTherapyBackendApp.Application.Dtos;
using DigitalTherapyBackendApp.Domain.Interfaces;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.PatientProfiles.Queries
{
    public class GetPatientsByPsychologistQuery : IRequest<GetPatientListResponse>
    {
        public Guid PsychologistId { get; }

        public GetPatientsByPsychologistQuery(Guid psychologistId)
        {
            PsychologistId = psychologistId;
        }
    }

    public class GetPatientsByPsychologistQueryHandler : IRequestHandler<GetPatientsByPsychologistQuery, GetPatientListResponse>
    {
        private readonly ITherapistPatientRelationshipRepository _relationshipRepository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly ILogger<GetPatientsByPsychologistQueryHandler> _logger;

        public GetPatientsByPsychologistQueryHandler(
            ITherapistPatientRelationshipRepository relationshipRepository,
            IPatientProfileRepository patientProfileRepository,
            ILogger<GetPatientsByPsychologistQueryHandler> logger)
        {
            _relationshipRepository = relationshipRepository;
            _patientProfileRepository = patientProfileRepository;
            _logger = logger;
        }

        public async Task<GetPatientListResponse> Handle(GetPatientsByPsychologistQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var relationships = await _relationshipRepository.GetByPsychologistIdAsync(request.PsychologistId);
                var patientProfiles = new List<PatientProfileDto>();

                foreach (var relationship in relationships)
                {
                    var patient = await _patientProfileRepository.GetByIdAsync(relationship.PatientId);
                    if (patient != null)
                    {
                        patientProfiles.Add(new PatientProfileDto
                        {
                            Id = patient.Id,
                            UserId = patient.UserId,
                            FirstName = patient.FirstName,
                            LastName = patient.LastName,
                            BirthDate = patient.BirthDate,
                            Gender = patient.Gender,
                            Bio = patient.Bio,
                            AvatarUrl = patient.AvatarUrl,
                            PreferredLanguage = patient.PreferredLanguage,
                            CountryCode = patient.CountryCode,
                            NotificationPreferences = patient.NotificationPreferences,
                            RelationshipStatus = relationship.Status,
                            RelationshipStartDate = relationship.StartDate
                        });
                    }
                }

                return new GetPatientListResponse
                {
                    Success = true,
                    Data = patientProfiles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients for psychologist: {PsychologistId}", request.PsychologistId);
                return new GetPatientListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the patients list."
                };
            }
        }
    }
}
