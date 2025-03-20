using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using DigitalTherapyBackendApp.Domain.Constants;
using DigitalTherapyBackendApp.Api.Features.Auth.Responses;
using DigitalTherapyBackendApp.Api.Features.Auth.Payloads;

namespace DigitalTherapyBackendApp.Api.Features.Auth.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public LoginPayload Payload { get; }

        public LoginCommand(LoginPayload payload)
        {
            Payload = payload;
        }
    }


    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRedisService _redisService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IJwtTokenService jwtTokenService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IRedisService redisService,
            ILogger<LoginCommandHandler> logger)
        {
            _jwtTokenService = jwtTokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _redisService = redisService;
            _logger = logger;
        }


        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Payload.UsernameOrEmail) ??
                           await _userManager.FindByEmailAsync(request.Payload.UsernameOrEmail);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {UsernameOrEmail}", request.Payload.UsernameOrEmail);
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "User not found.",
                        ErrorCode = ErrorCodes.UserNotFound
                    };
                }

                // Password Control
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Payload.Password, true);
                if (!signInResult.Succeeded)
                {
                    _logger.LogWarning("Failed login attempt. UserId: {UserId}", user.Id);
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "The username or password is incorrect.",
                        ErrorCode = ErrorCodes.UsernamePasswordIncorrect
                    };
                }

                // Create Token
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtTokenService.GenerateToken(user.Id, user.UserName, roles.FirstOrDefault());
                var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);

                await _redisService.SetAsync(
                    $"user:{user.Id}:refresh_token",
                    refreshToken,
                    TimeSpan.FromDays(14) // 14 gün
                );

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    ErrorCode = string.Empty,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(120),
                    User = new UserData
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        Role = new RoleData
                        {
                            RoleId = user.RoleId,
                            RoleName = roles.FirstOrDefault()
                        },
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the login request.");
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred: " + ex.Message.ToString(),
                    ErrorCode = ErrorCodes.UserCreationError
                };
            }
        }
    }
}
