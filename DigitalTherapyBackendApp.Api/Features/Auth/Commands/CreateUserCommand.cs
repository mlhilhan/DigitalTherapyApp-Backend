using DigitalTherapyBackendApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using DigitalTherapyBackendApp.Domain.Constants;
using DigitalTherapyBackendApp.Api.Features.Auth.Responses;
using DigitalTherapyBackendApp.Api.Features.Auth.Payloads;
using DigitalTherapyBackendApp.Domain.Interfaces;

namespace DigitalTherapyBackendApp.Api.Features.Auth.Commands
{
    public class CreateUserCommand : IRequest<CreateUserResponse>
    {
        public CreateUserPayload Payload { get; }

        public CreateUserCommand(CreateUserPayload payload)
        {
            Payload = payload;
        }
    }


    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<User> _logger;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IPsychologistProfileRepository _psychologistProfileRepository;
        private readonly IInstitutionProfileRepository _institutionProfileRepository;

        public CreateUserCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<User> logger, IPatientProfileRepository patientProfileRepository, IPsychologistProfileRepository psychologistProfileRepository, IInstitutionProfileRepository institutionProfileRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _patientProfileRepository = patientProfileRepository;
            _psychologistProfileRepository = psychologistProfileRepository;
            _institutionProfileRepository = institutionProfileRepository;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Username Control
                var existingUser = await _userManager.FindByNameAsync(request.Payload.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning("This username is already in use: {Username}", request.Payload.Username);
                    return CreateErrorResponse("This username is already in use.", ErrorCodes.UsernameAlreadyExists);
                }

                // Email Control
                var existingUserByEmail = await _userManager.FindByEmailAsync(request.Payload.Email);
                if (existingUserByEmail != null)
                {
                    _logger.LogWarning("This email is already in use: {Email}", request.Payload.Email);
                    return CreateErrorResponse("This email is already in use.", ErrorCodes.EmailAlreadyExists);
                }

                // Role Control
                var role = await _roleManager.FindByIdAsync(request.Payload.RoleId.ToString());
                if (role == null)
                {
                    _logger.LogWarning("Invalid role ID: {RoleId}", request.Payload.RoleId);
                    return CreateErrorResponse("Invalid role ID.", ErrorCodes.InvalidRoleId);
                }

                var user = new User
                {
                    UserName = request.Payload.Username,
                    Email = request.Payload.Email,
                    RoleId = request.Payload.RoleId,
                    CreatedAt = DateTime.UtcNow
                };

                // User Create
                var result = await _userManager.CreateAsync(user, request.Payload.Password);
                if (result.Succeeded)
                {
                    if (request.Payload.RoleId.ToString() == "4b41d3bc-95cb-4758-8c01-c5487707931e")
                    {
                        var patientprofile = new PatientProfile
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            User = user,
                        };

                        await _patientProfileRepository.AddAsync(patientprofile);
                    }

                    if (request.Payload.RoleId.ToString() == "40c2b39a-a133-4ba9-a97b-ce351bd101ac")
                    {
                        var psychologistProfile = new PsychologistProfile
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            User = user,
                        };

                        await _psychologistProfileRepository.AddAsync(psychologistProfile);
                    }

                    if (request.Payload.RoleId.ToString() == "5e6ef66e-8298-4002-b765-5a794f149362")
                    {
                        var institutionProfile = new InstitutionProfile
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            User = user,
                        };

                        await _institutionProfileRepository.AddAsync(institutionProfile);
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create user: {Errors}", errors);
                    return CreateErrorResponse($"User could not be created: {errors}", ErrorCodes.UserCreationFailed);
                }

                // Role Assignment
                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, role.Name);
                if (!roleAssignmentResult.Succeeded)
                {
                    var errors = string.Join(", ", roleAssignmentResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to assign role to user: {Errors}", errors);
                    return CreateErrorResponse($"Role assignment failed: {errors}", ErrorCodes.RoleAssignmentFailed);
                }

                return new CreateUserResponse
                {
                    Success = true,
                    Message = "User created successfully.",
                    ErrorCode = null,
                    Data = new UserData
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        Role = new RoleData
                        {
                            RoleId = role.Id,
                            RoleName = role.Name
                        },
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the user.");
                return CreateErrorResponse($"An error occurred: {ex.Message}", ErrorCodes.UserCreationError);
            }
        }

        private CreateUserResponse CreateErrorResponse(string message, string errorCode)
        {
            return new CreateUserResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }
    }
}
