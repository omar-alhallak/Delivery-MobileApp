using DeliveryApp.Domain.Entities.Customers.Order;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.CustomersConfiguration.OrderConfiguration
{
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", "Customers");

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
            //         Relations
            // -------------------------

            builder.Property(x => x.OrderID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            // -------------------------
            //         Snapshots
            // -------------------------

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
            //       Relationships
            // -------------------------

            builder.HasOne<Order>()
                .WithMany("items")
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.OrderID);
        }
    }
}