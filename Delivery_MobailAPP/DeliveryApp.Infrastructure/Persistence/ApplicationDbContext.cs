using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.Entities.Drivers;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Customers;
using DeliveryApp.Domain.Entities.Moderation;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using DeliveryApp.Domain.Entities.Merchants.Catalog;

namespace DeliveryApp.Infrastructure.Persistence
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // -------------------------
        //         Customers
        // -------------------------

        public DbSet<Address> Addresses => Set<Address>();

        // -------------------------
        //           Orders
        // -------------------------

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderAssignment> OrderAssignments => Set<OrderAssignment>();

        // -------------------------
        //          Drivers
        // -------------------------

        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<DriverRequest> DriverRequests => Set<DriverRequest>();
        public DbSet<DriverLocation> DriverLocations => Set<DriverLocation>();

        public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();

        // -------------------------
        //        Engagements
        // -------------------------

        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<Notification> Notifications => Set<Notification>();

        // -------------------------
        //          Identity
        // -------------------------

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();

        // -------------------------
        //          Merchants
        // -------------------------

        public DbSet<Merchant> Merchants => Set<Merchant>();
        public DbSet<MerchantUser> MerchantUsers => Set<MerchantUser>();
        public DbSet<MerchantWorkingHour> MerchantWorkingHours => Set<MerchantWorkingHour>();

        // -------------------------
        //          Catalog
        // -------------------------

        public DbSet<SystemCategory> SystemCategories => Set<SystemCategory>();
        public DbSet<MerchantSystemCategory> MerchantSystemCategories => Set<MerchantSystemCategory>();

        public DbSet<MerchantCategory> MerchantCategories => Set<MerchantCategory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Variant> Variants => Set<Variant>();

        // -------------------------
        //         Moderation
        // -------------------------

        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<AccountWarning> AccountWarnings => Set<AccountWarning>();

        // -------------------------
        //          Zones
        // -------------------------

        public DbSet<Zone> Zones => Set<Zone>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) // تعريف شكل قاعدة البيانات قبل ما تنبنى
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            //         Sequences
            // -------------------------

            // عداد ثابت بمقدار 1 ينشأ مباشرة في القاعدة
            // Example: First PublicID = U-000001 - Second PublicID = U-000002 ----> to infinity

            modelBuilder.HasSequence<long>("user_public_seq", "shared")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.HasSequence<long>("order_public_seq", "shared")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.HasSequence<long>("merchant_public_seq", "shared")
                .StartsAt(1)
                .IncrementsBy(1);

            // -------------------------
            //      Configurations
            // -------------------------

            // يبحث عن كل الكونغوريشن الموجودة في المشروع ويطبقها تلقائياً على الجداول
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}