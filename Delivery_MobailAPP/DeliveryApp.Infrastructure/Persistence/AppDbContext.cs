using DeliveryApp.Domain.Entities.Customers;
using DeliveryApp.Domain.Entities.Customers.Order;
using DeliveryApp.Domain.Entities.Drivers;
using DeliveryApp.Domain.Entities.Feedback;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Domain.Entities.Moderation;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserIdentity> UserIdentities { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<MerchantUser> MerchantUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderDriver> OrderDrivers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverRequest> DriverRequests { get; set; }
        public DbSet<DriverLocation> DriverLocations { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AccountWarning> AccountWarnings { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<ZonePolygon> ZonePolygons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MerchantUser>().HasKey(mu => new { mu.MerchantID, mu.UserID });

            modelBuilder.Entity<OrderDriver>().HasKey(od => od.ID);

            modelBuilder.Entity<OrderItem>().HasKey(oi => oi.OrderItemID);
        }
    }
}