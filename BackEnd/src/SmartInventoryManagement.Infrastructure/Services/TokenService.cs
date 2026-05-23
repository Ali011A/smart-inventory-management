using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartInventoryManagement.Application.Interfaces.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventoryManagement.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(
            string userId,
            string email,
            IEnumerable<string> roles)
        {
            var secret = _config["Jwt:Secret"]
                ?? throw new InvalidOperationException(
                    "JWT secret is not configured.");

            if (secret.Length < 32)
                throw new InvalidOperationException(
                    "JWT secret must be at least 32 characters.");

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),

            new(ClaimTypes.Email, email),

            // Unique token id
            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

            // Add roles to claims
            claims.AddRange(
                roles.Select(role =>
                    new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
