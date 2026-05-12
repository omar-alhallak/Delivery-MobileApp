using DeliveryApp.Application.Features.Identity.LoginLocal;
using DeliveryApp.Application.Features.Identity.RegisterLocal;
using DeliveryApp.Application.Interfaces;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;
using DeliveryApp.Infrastructure.Implementation;
using DeliveryApp.Infrastructure.Implementation.Identity;
using DeliveryApp.Infrastructure.Options;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            // توليد أكواد العامة
            services.AddScoped<IPublicCodeGenerator, SqlServerPublicCodeGenerator>();

            // تشفير كلمة السر
            services.AddScoped<IPasswordHasher, PasswordHasherService>();

            // 
            services.AddScoped<IRegisterLocalRepository, RegisterLocalRepository>();

            // 
            services.AddScoped<ILoginLocalRepository, LoginLocalRepository>();

            // 
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<RegisterLocalService>();
            services.AddScoped<LoginLocalService>();

            return services;
        }
    }
}