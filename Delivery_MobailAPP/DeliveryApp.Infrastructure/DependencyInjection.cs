using DeliveryApp.Application.Features.Addresses.AddressLifecycle;
using DeliveryApp.Application.Features.Addresses.AddressQuery;
using DeliveryApp.Application.Features.Addresses.AddressStatus;
using DeliveryApp.Application.Features.Identity.LoginLocal;
using DeliveryApp.Application.Features.Identity.Logout;
using DeliveryApp.Application.Features.Identity.RefreshToken;
using DeliveryApp.Application.Features.Identity.RegisterLocal;
using DeliveryApp.Application.Features.Identity.UpdateMyProfile;
using DeliveryApp.Application.Features.Media.UploadMedia;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Features.MerchantCatalog.MerchantCategories;
using DeliveryApp.Application.Features.MerchantCatalog.MerchantSystemCategories;
using DeliveryApp.Application.Features.MerchantCatalog.Products;
using DeliveryApp.Application.Features.MerchantCatalog.PublicCatalog;
using DeliveryApp.Application.Features.MerchantCatalog.SystemCategories;
using DeliveryApp.Application.Features.MerchantCatalog.Variants;
using DeliveryApp.Application.Features.Merchants.ActivateMerchant;
using DeliveryApp.Application.Features.Merchants.AddMerchantStaff;
using DeliveryApp.Application.Features.Merchants.CreateMerchant;
using DeliveryApp.Application.Features.Merchants.GetMerchantDetails;
using DeliveryApp.Application.Features.Merchants.GetMerchants;
using DeliveryApp.Application.Features.Merchants.GetMerchantStaff;
using DeliveryApp.Application.Features.Merchants.GetWorkingHours;
using DeliveryApp.Application.Features.Merchants.RemoveMerchantStaff;
using DeliveryApp.Application.Features.Merchants.SetWorkingHours;
using DeliveryApp.Application.Features.Merchants.UpdateMerchant;
using DeliveryApp.Application.Features.Notifications;
using DeliveryApp.Application.Features.Orders.Access;
using DeliveryApp.Application.Features.Orders.CancelOrder;
using DeliveryApp.Application.Features.Orders.CreateOrder;
using DeliveryApp.Application.Features.Orders.DeleteOrder;
using DeliveryApp.Application.Features.Orders.GetOrders;
using DeliveryApp.Application.Features.Orders.MerchantDecision;
using DeliveryApp.Application.Features.Orders.OrderWorkflow;
using DeliveryApp.Application.Features.Orders.Payment;
using DeliveryApp.Application.Features.Ratings;
using DeliveryApp.Application.Interfaces;
using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;
using DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces;
using DeliveryApp.Application.Interfaces.MediaInterfaces;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using DeliveryApp.Application.Interfaces.NotificationRepositoriesInterfaces;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Application.Interfaces.RatingRepositoriesInterfaces;
using DeliveryApp.Application.Interfaces.Services;
using DeliveryApp.Infrastructure.Implementation.AddressRepositories;
using DeliveryApp.Infrastructure.Implementation.IdentityRepositores;
using DeliveryApp.Infrastructure.Implementation.MerchantCatalogRepositories;
using DeliveryApp.Infrastructure.Implementation.Merchants.Repositores;
using DeliveryApp.Infrastructure.Implementation.NotificationRepositores;
using DeliveryApp.Infrastructure.Implementation.OrderRepositores;
using DeliveryApp.Infrastructure.Implementation.RatingRepositores;
using DeliveryApp.Infrastructure.Implementation.Services;
using DeliveryApp.Infrastructure.Options;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Infrastructure.Repositories.MerchantRepositories;
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

            // ---------- Media Services ----------

            services.AddScoped<UploadMediaService>();

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

            services.AddScoped<IOrderAccessRepository, OrderAccessRepository>();

            // ---------- Merchant Repositories ----------

            services.AddScoped<ICreateMerchantRepository, CreateMerchantRepository>();

            services.AddScoped<IUpdateMerchantRepository, UpdateMerchantRepository>();

            services.AddScoped<ISetMerchantWorkingHoursRepository, SetMerchantWorkingHoursRepository>();

            services.AddScoped<IActivateMerchantRepository, ActivateMerchantRepository>();

            services.AddScoped<IGetMerchantsRepository, GetMerchantsRepository>();

            services.AddScoped<IAddMerchantStaffRepository, AddMerchantStaffRepository>();

            services.AddScoped<IRemoveMerchantStaffRepository, RemoveMerchantStaffRepository>();

            services.AddScoped<IGetMerchantWorkingHoursRepository, GetMerchantWorkingHoursRepository>();

            services.AddScoped<IGetMerchantStaffRepository, GetMerchantStaffRepository>();

            services.AddScoped<IGetMerchantDetailsRepository, GetMerchantDetailsRepository>();

            //---------- Catagories Repositories ----------

            services.AddScoped<IMerchantCatalogReadRepository, MerchantCatalogReadRepository>();

            services.AddScoped<IMerchantCatalogCommandRepository, MerchantCatalogCommandRepository>();

            services.AddScoped<IMerchantCatalogAccessRepository, MerchantCatalogAccessRepository>();

            //---------- Rating Repositories ----------

            services.AddScoped<IRatingReadRepository, RatingReadRepository>();

            services.AddScoped<IRatingCommandRepository, RatingCommandRepository>();

            //---------- Notification Repositories ----------

            services.AddScoped<INotificationReadRepository, NotificationReadRepository>();

            services.AddScoped<INotificationCommandRepository, NotificationCommandRepository>();

            //---------- Address Repositories ----------

            services.AddScoped<IAddressReadRepository, AddressReadRepository>();

            services.AddScoped<IAddressCommandRepository, AddressCommandRepository>();

            //---------- Media Repositories ----------

            services.AddScoped<IMediaStorage, LocalMediaStorage>();

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

            services.AddScoped<OrderAccessService>();

            // ---------- Merchant Features ----------

            services.AddScoped<CreateMerchantService>();

            services.AddScoped<UpdateMerchantService>();

            services.AddScoped<SetMerchantWorkingHoursService>();

            services.AddScoped<ActivateMerchantService>();

            services.AddScoped<GetMerchantsService>();

            services.AddScoped<AddMerchantStaffService>();

            services.AddScoped<RemoveMerchantStaffService>();

            services.AddScoped<GetMerchantWorkingHoursService>();

            services.AddScoped<GetMerchantStaffService>();

            services.AddScoped<GetMerchantDetailsService>();

            // ---------- Catagories Features ----------

            services.AddScoped<SystemCategoryService>();

            services.AddScoped<MerchantSystemCategoryService>();

            services.AddScoped<MerchantCategoryService>();

            services.AddScoped<ProductService>();

            services.AddScoped<VariantService>();

            services.AddScoped<PublicCatalogService>();

            services.AddScoped<MerchantCatalogAccessService>();

            // ---------- Rating Features ----------

            services.AddScoped<RatingService>();

            // ---------- Notification Features ----------

            services.AddScoped<NotificationService>();

            // ---------- Address Features ----------

            services.AddScoped<AddressQueryService>();

            services.AddScoped<AddressLifecycleService>();

            services.AddScoped<AddressStatusService>();

            // -----------------------------------

            return services;
        }
    }
}