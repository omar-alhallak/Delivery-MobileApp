using DeliveryApp.Domain.Entities.Customers.Order;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Merchants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Orders;

namespace DeliveryApp.Infrastructure.Configuration.CustomersConfiguration.OrderConfiguration
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "Customers");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Public Code
            // -------------------------

            builder.Property(x => x.PublicID)
                .HasConversion(
                    value => value.HasValue ? value.Value.Value : null,
                    value => string.IsNullOrWhiteSpace(value) ? null : PublicCode.From(value))
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsRequired(false);

            // -------------------------
            //       Order Details
            // -------------------------

            builder.Property(x => x.OrderType)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.CustomerID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.Property(x => x.MerchantID)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<MerchantTag>.From(value.Value) : null)
                .IsRequired(false);

            // -------------------------
            //         Locations
            // -------------------------

            builder.OwnsOne(x => x.PickupLocation, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("PickupLatitude")
                    .HasPrecision(9, 6)
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("PickupLongitude")
                    .HasPrecision(9, 6)
                    .IsRequired();
            });

            builder.OwnsOne(x => x.DropoffLocation, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("DropoffLatitude")
                    .HasPrecision(9, 6)
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("DropoffLongitude")
                    .HasPrecision(9, 6)
                    .IsRequired();
            });

            // -------------------------
            //         Snapshots
            // -------------------------

            builder.Property(x => x.DistanceKmSnapshot)
                .HasPrecision(9, 2)
                .IsRequired();

            builder.Property(x => x.ItemsTotalSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.DeliveryFeeSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.TipAmountSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.TotalAmountSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            // -------------------------
            //          Payment
            // -------------------------

            builder.Property(x => x.PaymentMethod)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //          Status
            // -------------------------

            builder.Property(x => x.Status)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.RequiredDriversCount)
                .IsRequired();

            // -------------------------
            //           Issue
            // -------------------------

            builder.Property(x => x.IssueReason)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.IssueNote)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------------------------
            //       Cancellation
            // -------------------------

            builder.Property(x => x.CancelledByType)
                .HasConversion<byte>()
                .IsRequired(false);

            builder.Property(x => x.CancelledById)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<UserTag>.From(value.Value) : null)
                .IsRequired(false);

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CancelledAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.ConfirmedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            builder.Property(x => x.DeliveredAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CancelledById)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Items
            // -------------------------

            builder.Metadata.FindNavigation(nameof(Order.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.PublicID)
                .IsUnique()
                .HasFilter("[PublicID] IS NOT NULL");

            builder.HasIndex(x => x.CustomerID);

            builder.HasIndex(x => x.MerchantID);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}