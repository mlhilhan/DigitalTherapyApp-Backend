using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Payloads;
using DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace DigitalTherapyBackendApp.Api.Features.PsychologistProfiles.Commands
{
    public class UpdatePsychologistSpecialtiesCommand : IRequest<UpdatePsychologistSpecialtiesResponse>
    {
        [JsonIgnore]
        public Guid PsychologistId { get; set; }

        public List<string> Specialties { get; set; }

        public static UpdatePsychologistSpecialtiesCommand FromPayload(UpdatePsychologistSpecialtiesPayload payload, Guid psychologistId)
        {
            return new UpdatePsychologistSpecialtiesCommand
            {
                PsychologistId = psychologistId,
                Specialties = payload.Specialties
            };
        }
    }

    public class UpdatePsychologistSpecialtiesCommandHandler : IRequestHandler<UpdatePsychologistSpecialtiesCommand, UpdatePsychologistSpecialtiesResponse>
    {
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly ILogger<UpdatePsychologistSpecialtiesCommandHandler> _logger;

        public UpdatePsychologistSpecialtiesCommandHandler(
            IPsychologistProfileRepository psychologistProfileRepository,
            ISpecialtyRepository specialtyRepository,
            ILogger<UpdatePsychologistSpecialtiesCommandHandler> logger)
        {
            _psychologistProfileRepository = psychologistProfileRepository;
            _specialtyRepository = specialtyRepository;
            _logger = logger;
        }

        public async Task<UpdatePsychologistSpecialtiesResponse> Handle(UpdatePsychologistSpecialtiesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var psychologist = await _psychologistProfileRepository.GetByIdAsync(request.PsychologistId);
                if (psychologist == null)
                {
                    _logger.LogWarning("Psychologist profile not found with ID: {PsychologistId}", request.PsychologistId);
                    return new UpdatePsychologistSpecialtiesResponse
                    {
                        Success = false,
                        Message = "No psychologist profile found."
                    };
                }

                await _psychologistProfileRepository.ClearSpecialtiesAsync(request.PsychologistId);

                var specialties = new List<Specialty>();
                foreach (var specialtyName in request.Specialties)
                {
                    var specialty = await _specialtyRepository.GetByNameAsync(specialtyName);
                    if (specialty == null)
                    {
                        specialty = new Specialty { Name = specialtyName };
                        specialty = await _specialtyRepository.AddAsync(specialty);
                    }
                    specialties.Add(specialty);
                }

                await _psychologistProfileRepository.UpdateSpecialtiesAsync(request.PsychologistId, specialties);

                return new UpdatePsychologistSpecialtiesResponse
                {
                    Success = true,
                    Data = request.Specialties,
                    Message = "Specializations have been updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating specialties for psychologist: {PsychologistId}", request.PsychologistId);
                return new UpdatePsychologistSpecialtiesResponse
                {
                    Success = false,
                    Message = "An error occurred while updating specializations: " + ex.Message
                };
            }
        }
    }
}