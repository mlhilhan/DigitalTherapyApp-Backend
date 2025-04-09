using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.PatientProfiles.Queries
{
    public class GetPatientsByInstitutionQuery : IRequest<GetPatientListResponse>
    {
        public Guid InstitutionId { get; }

        public GetPatientsByInstitutionQuery(Guid institutionId)
        {
            InstitutionId = institutionId;
        }
    }

    public class GetPatientsByInstitutionQueryHandler : IRequestHandler<GetPatientsByInstitutionQuery, GetPatientListResponse>
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IPsychologistProfileRepository _psychologistRepository;
        private readonly ITherapistPatientRelationshipRepository _relationshipRepository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly ILogger<GetPatientsByInstitutionQueryHandler> _logger;

        public GetPatientsByInstitutionQueryHandler(
            IInstitutionRepository institutionRepository,
            IPsychologistProfileRepository psychologistRepository,
            ITherapistPatientRelationshipRepository relationshipRepository,
            IPatientProfileRepository patientProfileRepository,
            ILogger<GetPatientsByInstitutionQueryHandler> logger)
        {
            _institutionRepository = institutionRepository;
            _psychologistRepository = psychologistRepository;
            _relationshipRepository = relationshipRepository;
            _patientProfileRepository = patientProfileRepository;
            _logger = logger;
        }

        public async Task<GetPatientListResponse> Handle(GetPatientsByInstitutionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var psychologists = await _psychologistRepository.GetByInstitutionIdAsync(request.InstitutionId);

                var uniquePatientIds = new HashSet<Guid>();
                var patientProfiles = new List<PatientProfileDto>();

                foreach (var psychologist in psychologists)
                {
                    var relationships = await _relationshipRepository.GetByPsychologistIdAsync(psychologist.Id);

                    foreach (var relationship in relationships)
                    {
                        if (!uniquePatientIds.Contains(relationship.PatientId))
                        {
                            uniquePatientIds.Add(relationship.PatientId);

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
                                    RelationshipStartDate = relationship.StartDate,
                                    PsychologistId = psychologist.Id,
                                    PsychologistName = $"{psychologist.FirstName} {psychologist.LastName}"
                                });
                            }
                        }
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
                _logger.LogError(ex, "Error retrieving patients for institution: {InstitutionId}", request.InstitutionId);
                return new GetPatientListResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving the patients list."
                };
            }
        }
    }
}
