using Microsoft.EntityFrameworkCore;
using DeliveryApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using DeliveryApp.Infrastructure.Options;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using DeliveryApp.Infrastructure.Implementation;
using DeliveryApp.Application.Features.Identity.Logout;
using DeliveryApp.Application.Features.Identity.LoginLocal;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;
using DeliveryApp.Application.Features.Identity.RefreshToken;
using DeliveryApp.Application.Features.Identity.RegisterLocal;
using DeliveryApp.Application.Features.Identity.UpdateMyProfile;
using DeliveryApp.Infrastructure.Implementation.Identity.Services;
using DeliveryApp.Infrastructure.Implementation.Identity.Repositores;

namespace DeliveryApp.Infrastructure // تسجيل جميع ميزات طبقة Infrastructure
{                                    // داخل الكلاس واحد لربطه مع Program.cs 
    public static class DependencyInjection // "بشكل مبسط: بوابة الربط بين طبقة"
    {                                       // "Infrastructure مع التطبيق"
        // IServiceCollection: إرجاع ميزات
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // للأتصال مع القاعدة
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

            // توليد القاعدة عن طريق ApplicationDbContext 
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            // -------------------------
            //         SERVICES
            // -------------------------

            // ---------- PublicCode Services ----------

            services.AddScoped<IPublicCodeGenerator, SqlServerPublicCodeGenerator>();

            // ---------- Identity Services ----------

            services.AddScoped<IPasswordHasher, PasswordHasherService>();

            // خدمات JWT
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddScoped<ITokenService, TokenService>();

            // -------------------------
            //       REPOSITORIES
            // -------------------------

            // ---------- Identity Repositories ----------
   
            services.AddScoped<IRegisterLocalRepository, RegisterLocalRepository>();

            services.AddScoped<ILoginLocalRepository, LoginLocalRepository>();

            services.AddScoped<IUpdateMyProfileRepository, UpdateMyProfileRepository>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<ILogoutRepository, LogoutRepository>();

            // -------------------------
            //         FEATURES
            // -------------------------

            // ---------- Identity Features ----------

            services.AddScoped<RegisterLocalService>();
 
            services.AddScoped<LoginLocalService>();

            services.AddScoped<UpdateMyProfileService>();

            services.AddScoped<RefreshTokenService>();

            services.AddScoped<LogoutService>();

            // ----------------------------------------

            return services;
        }
    }
}