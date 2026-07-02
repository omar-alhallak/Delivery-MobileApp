using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DeliveryApp.API.Extensions
{
    // تجهيز API:
    //   A_ ليصبح يفهم JWT
    //   B_ ويعمل [Authorize]
    //   C_ فحص كامل لل Token
    public static class AuthenticationExtensions // مثل: نظام تحقق قبل الدخول
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // قراة إعدادات JWT
            var jwtSection = configuration.GetSection("Jwt");

            // قراة المفتاح السري
            var secretKey = jwtSection["SecretKey"] ?? throw new Exception("Jwt SecretKey is missing.");

            if (secretKey.Length < 32)
                throw new InvalidOperationException("Jwt SecretKey must be at least 32 characters");

            // تنصيب نظام القراءة الأفتراضي وجعله JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // القواعد لفحص Token
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ClockSkew = TimeSpan.Zero,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}