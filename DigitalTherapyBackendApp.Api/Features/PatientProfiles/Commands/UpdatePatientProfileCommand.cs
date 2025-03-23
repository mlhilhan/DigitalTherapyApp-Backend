using DigitalTherapyBackendApp.Api.Features.PatientProfiles.Responses;
using DigitalTherapyBackendApp.Domain.Entities;
using DigitalTherapyBackendApp.Domain.Interfaces;
using DigitalTherapyBackendApp.Application.Dtos;
using MediatR;

namespace DigitalTherapyBackendApp.Api.Features.PatientProfiles.Commands
{
    public class UpdatePatientProfileCommand : IRequest<GetPatientProfileResponse>
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? NotificationPreferences { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class UpdatePatientProfileCommandHandler : IRequestHandler<UpdatePatientProfileCommand, GetPatientProfileResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly ILogger<UpdatePatientProfileCommandHandler> _logger;

        public UpdatePatientProfileCommandHandler(
            IUserRepository userRepository,
            IPatientProfileRepository patientProfileRepository,
            ILogger<UpdatePatientProfileCommandHandler> logger)
        {
            _userRepository = userRepository;
            _patientProfileRepository = patientProfileRepository;
            _logger = logger;
        }

        public async Task<GetPatientProfileResponse> Handle(UpdatePatientProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _patientProfileRepository.GetByUserIdAsync(request.UserId);
                if (profile == null)
                {
                    profile = new PatientProfile
                    {
                        UserId = request.UserId
                    };
                }

                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user != null)
                {
                    user.Email = request.Email;
                    user.PhoneNumber = request.PhoneNumber;
                    await _userRepository.UpdateAsync(user);
                }

                profile.FirstName = request.FirstName;
                profile.LastName = request.LastName;
                profile.BirthDate = request.BirthDate;
                profile.Gender = request.Gender;
                profile.Bio = request.Bio;
                profile.PreferredLanguage = request.PreferredLanguage;
                profile.NotificationPreferences = request.NotificationPreferences;
                    

                if (profile.Id == Guid.Empty)
                {
                    profile = await _patientProfileRepository.AddAsync(profile);
                }
                else
                {
                    profile = await _patientProfileRepository.UpdateAsync(profile);
                }

                return new GetPatientProfileResponse
                {
                    Success = true,
                    Message = "Patient profile updated successfully.",
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
                        NotificationPreferences = profile.NotificationPreferences,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient profile for user: {UserId}", request.UserId);
                return new GetPatientProfileResponse
                {
                    Success = false,
                    Message = "An error occurred while updating the patient profile."
                };
            }
        }
    }
}
