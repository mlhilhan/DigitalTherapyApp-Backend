using DigitalTherapyBackendApp.Api.Features.Auth.Payloads;
using DigitalTherapyBackendApp.Api.Features.Auth.Responses;
using DigitalTherapyBackendApp.Application.Interfaces;
using DigitalTherapyBackendApp.Infrastructure.Repositories;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DigitalTherapyBackendApp.Domain.Interfaces;

namespace DigitalTherapyBackendApp.Api.Features.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public RefreshTokenCommand(RefreshTokenPayload payload)
        {
            AccessToken = payload.AccessToken;
            RefreshToken = payload.RefreshToken;
        }
    }


    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IRedisService _redisService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IUserRepository _userRepository;

        public RefreshTokenCommandHandler(
            IRedisService redisService,
            IJwtTokenService jwtTokenService,
            TokenValidationParameters tokenValidationParameters,
            IUserRepository userRepository)
        {
            _redisService = redisService;
            _jwtTokenService = jwtTokenService;
            _tokenValidationParameters = tokenValidationParameters;
            _userRepository = userRepository;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
                throw new UnauthorizedAccessException("Invalid access token");

            var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(subClaim))
            {
                throw new UnauthorizedAccessException("Token does not contain required 'sub' claim");
            }

            var userId = Guid.Parse(subClaim);
            // var userId = Guid.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);
            var username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;

            var storedRefreshToken = await _redisService.GetAsync($"user:{userId}:refresh_token");

            if (string.IsNullOrEmpty(storedRefreshToken) || storedRefreshToken != request.RefreshToken)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (!string.IsNullOrEmpty(jti))
            {
                await _redisService.SetAsync(
                    $"blacklist:token:{jti}",
                    "revoked",
                    TimeSpan.FromMinutes(120)
                );
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var newAccessToken = _jwtTokenService.GenerateToken(userId, username, user.Role.Name);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken(userId);

            await _redisService.SetAsync(
                $"user:{userId}:refresh_token",
                newRefreshToken,
                TimeSpan.FromDays(14)
            );

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(120)
            };
        }

        private ClaimsPrincipal GetPrincipalFromExpiredTokenX(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = _tokenValidationParameters.ValidateAudience,
                ValidateIssuer = _tokenValidationParameters.ValidateIssuer,
                ValidAudience = _tokenValidationParameters.ValidAudience,
                ValidIssuer = _tokenValidationParameters.ValidIssuer,
                ValidateIssuerSigningKey = _tokenValidationParameters.ValidateIssuerSigningKey,
                IssuerSigningKey = _tokenValidationParameters.IssuerSigningKey,
                ValidateLifetime = false // Önemli: Token süresi geçmiş olsa bile doğrulayabilmek için
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken == null ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch
            {
                return null;
            }
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Token cannot be null or empty");
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (!tokenHandler.CanReadToken(token))
                {
                    throw new SecurityTokenException("Invalid token format");
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = _tokenValidationParameters.ValidateIssuer,
                    ValidIssuer = _tokenValidationParameters.ValidIssuer,
                    ValidateAudience = _tokenValidationParameters.ValidateAudience,
                    ValidAudience = _tokenValidationParameters.ValidAudience,
                    ValidateIssuerSigningKey = _tokenValidationParameters.ValidateIssuerSigningKey,
                    IssuerSigningKey = _tokenValidationParameters.IssuerSigningKey,
                    ValidateLifetime = false
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtToken) ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token algorithm");
                }

                if (principal.FindFirst(JwtRegisteredClaimNames.Sub) == null)
                {
                    throw new SecurityTokenException("Token must contain 'sub' claim");
                }

                return principal;
            }
            catch (SecurityTokenException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Token validation failed", ex);
            }
        }
    }
}
