using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DeliveryApp.Infrastructure.Options;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;

// JWT:    مؤلف من 3 أقسام
// Header: فيه معلومات عن نوع التوكن و الخوارزمية     // alg: HMACSHA256
// Payload: البيانات داخل التوكن
// Signature: التوقيع الي بيتأكد من صحة التوكن
// Sig: يكون Header + Payload + Signature لو تغير حرف التوقيع يفشل
namespace DeliveryApp.Infrastructure.Implementation.Identity.Services
{
    public sealed class TokenService : ITokenService // الكلاس المسؤول عن Tokens
    {
        private const int MinSecretKeyLength = 32; // أقل حد للمفتاح السري

        private static readonly JwtSecurityTokenHandler TokenHandler = new();

        private readonly JwtOptions _options;

        public TokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));

            ValidateOptions(_options);
        }

        public string GenerateAccessToken(User user) // إنشاء للمستخدم JWT
        {
            if (user is null) 
                throw new ArgumentNullException(nameof(user));

            if (user.PublicID is null)
                throw new InvalidOperationException("User public id is required to generate access token.");

            // تحويل المفتاح السري ل بايت 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            // إنشاء التوقيع
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // claim: معلومات عن المستخدم مشان ما كل عملية أطلب من القاعدة 
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.ID.Value.ToString()),
                new("public_id", user.PublicID.Value.Value),
                new("is_profile_complete", user.IsProfileComplete.ToString())
            };

            // إنشاء JWT
            var token = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes),
                signingCredentials: credentials
            );

            return TokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken() // ينشئ 64 بايت عشوائي
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public byte[] HashRefreshToken(string refreshToken) // تشفير توكن
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token is required.", nameof(refreshToken));

            using var sha256 = SHA256.Create();

            return sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        }
     
        public TimeSpan GetRefreshTokenLifetime() => TimeSpan.FromDays(_options.RefreshTokenDays); // مدة Session

        private static void ValidateOptions(JwtOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.SecretKey))
                throw new InvalidOperationException("Jwt SecretKey is required.");

            if (options.SecretKey.Length < MinSecretKeyLength)
                throw new InvalidOperationException($"Jwt SecretKey must be at least {MinSecretKeyLength} characters.");

            if (options.AccessTokenMinutes <= 0)
                throw new InvalidOperationException("Jwt AccessTokenMinutes must be greater than zero.");

            if (options.RefreshTokenDays <= 0)
                throw new InvalidOperationException("Jwt RefreshTokenDays must be greater than zero.");
        }
    }
}