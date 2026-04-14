using Microsoft.EntityFrameworkCore;
using DeliveryApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using DeliveryApp.Infrastructure.Implementation;

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

            return services;
        }
    }
}