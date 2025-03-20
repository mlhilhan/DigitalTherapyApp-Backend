using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalTherapyBackendApp.Application.Interfaces;

namespace DigitalTherapyBackendApp.Infrastructure.Identity
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _secretKey;

        public JwtTokenService(IConfiguration configuration)
        {
            _secretKey = configuration["JWTSecurity:SecretKey"]
                         ?? throw new ArgumentNullException("JWT Secret Key is missing!");
        }

        public string GenerateToken(Guid userId, string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expirationTime = DateTime.UtcNow.AddMinutes(120);

            var token = new JwtSecurityToken(
                issuer: "DigitalTherapyApp",
                audience: "DigitalTherapyUsers",
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(Guid userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expirationTime = DateTime.UtcNow.AddDays(30);

            var refreshToken = new JwtSecurityToken(
                issuer: "DigitalTherapyApp",
                audience: "DigitalTherapyUsers",
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(refreshToken);
        }
    }
}
