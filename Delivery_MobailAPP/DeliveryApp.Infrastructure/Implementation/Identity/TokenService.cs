using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;
using DeliveryApp.Infrastructure.Options;

namespace DeliveryApp.Infrastructure.Implementation.Identity
{
    public sealed class TokenService : ITokenService
    {
        private readonly JwtOptions _options;

        public TokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.ID.Value.ToString()),
                new("public_id", user.PublicID!.Value.Value),
                new("is_profile_complete", user.IsProfileComplete.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));
        }

        public byte[] HashRefreshToken(string refreshToken)
        {
            using var sha256 = SHA256.Create();

            return sha256.ComputeHash(
                Encoding.UTF8.GetBytes(refreshToken));
        }
    }
}