using DeliveryApp.Application.Features.Identity.LoginLocal;
using DeliveryApp.Application.Features.Identity.Logout;
using DeliveryApp.Application.Features.Identity.RefreshToken;
using DeliveryApp.Application.Features.Identity.RegisterLocal;
using DeliveryApp.Application.Features.Identity.UpdateMyProfile;
using DeliveryApp.Application.Features.Orders.CancelOrder;
using DeliveryApp.Application.Features.Orders.CreateOrder;
using DeliveryApp.Application.Features.Orders.DeleteOrder;
using DeliveryApp.Application.Features.Orders.GetOrders;
using DeliveryApp.Application.Features.Orders.MerchantDecision;
using DeliveryApp.Application.Features.Orders.OrderWorkflow;
using DeliveryApp.Application.Features.Orders.Payment;
using DeliveryApp.Application.Interfaces;
using DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Application.Interfaces.Services;
using DeliveryApp.Infrastructure.Implementation.IdentityRepositores;
using DeliveryApp.Infrastructure.Implementation.OrderRepositores;
using DeliveryApp.Infrastructure.Implementation.Services;
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

            // ---------- Order Repositories ----------

            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderCreateRepository, OrderCreateRepository>();
            services.AddScoped<IOrderCommandRepository, OrderCommandRepository>();

            // -------------------------
            //         FEATURES
            // -------------------------

            // ---------- Identity Features ----------

            services.AddScoped<RegisterLocalService>();
 
            services.AddScoped<LoginLocalService>();

            services.AddScoped<UpdateMyProfileService>();

            services.AddScoped<RefreshTokenService>();

            services.AddScoped<LogoutService>();

            // ---------- Order Features ----------

            services.AddScoped<OrderQueryService>();
            services.AddScoped<CreateOrderService>();
            services.AddScoped<DeleteOrderService>();
            services.AddScoped<OrderWorkflowService>();
            services.AddScoped<MerchantDecisionService>();
            services.AddScoped<CancelOrderService>();
            services.AddScoped<OrderPaymentService>();


            return services;
        }
    }
}