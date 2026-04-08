using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Merchants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.CustomersConfiguration.OrderConfiguration
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "customers");

            // -------------------------
            //           Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //         Public ID
            // -------------------------

            builder.Property(x => x.PublicID)
                .HasConversion(
                    value => value.HasValue ? value.Value.Value : null,
                    value => string.IsNullOrWhiteSpace(value) ? null : PublicCode.From(value))
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsRequired(false);

            // -------------------------
            //       Foreign Keys
            // -------------------------

            builder.Property(x => x.CustomerID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.MerchantID)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<MerchantTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.CancelledById)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<UserTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CancelledById)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //            Enums
            // -------------------------

            builder.Property(x => x.OrderType)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.PaymentMethod)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.IssueReason)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.CancelledByType)
                .HasConversion<byte>()
                .IsRequired(false);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Locations --------

            builder.OwnsOne(x => x.PickupLocation, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("PickupLatitude")
                    .HasColumnType("float")
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("PickupLongitude")
                    .HasColumnType("float")
                    .IsRequired();
            });

            builder.OwnsOne(x => x.DropoffLocation, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("DropoffLatitude")
                    .HasColumnType("float")
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("DropoffLongitude")
                    .HasColumnType("float")
                    .IsRequired();
            });

            // -------- Snapshots --------

            builder.Property(x => x.DistanceKmSnapshot)
                .HasColumnType("float")
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

            // -------- Drivers --------

            builder.Property(x => x.RequiredDriversCount)
                .IsRequired();

            // -------- Issue --------

            builder.Property(x => x.IssueNote)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------- Dates --------

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
            //            Items
            // -------------------------

            builder.Metadata.FindNavigation(nameof(Order.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.PublicID)
                .IsUnique()
                .HasFilter("[PublicID] IS NOT NULL");

            builder.HasIndex(x => new 
            { 
                x.CustomerID, 
                x.CreatedAt 
            });

            builder.HasIndex(x => new 
            { 
                x.MerchantID,
                x.Status 
            })  .HasFilter("[MerchantID] IS NOT NULL");

            builder.HasIndex(x => new 
            { 
                x.Status, 
                x.CreatedAt 
            });
        }
    }
}