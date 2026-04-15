using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Customers.Orders;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.CustomersConfiguration.OrderConfiguration
{
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItem", "customers");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderItemTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.OrderID) // One(Order) -----> Many(OrderItem) || لأي طلب تابع هاد العنصر
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            builder.HasOne<Order>()
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Snapshots --------

            builder.Property(x => x.ProductNameSnapshot)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.VariantNameSnapshot)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.UnitPriceSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.LineTotalSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.CustomerNote)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.OrderID); // جلب عناصر طلب معين
        }
    }
}