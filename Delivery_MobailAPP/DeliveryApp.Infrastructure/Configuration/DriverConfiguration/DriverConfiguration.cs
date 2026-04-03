using DeliveryApp.Domain.Entities.Drivers;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Infrastructure.Configuration.DriverConfiguration
{
    public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {

            builder.ToTable("Drivers");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //          Vehicle
            // -------------------------

            builder.Property(x => x.VehicleTypeID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<VehicleTypeTag>.From(value))
                .IsRequired();

            // -------------------------
            //      Admin Control
            // -------------------------

            builder.Property(x => x.IsEnabled)
                .IsRequired();

                builder.Property(x => x.DisabledByAdminID)
                    .HasConversion(
                      id => id.HasValue ? id.Value.Value : (Guid?)null,
                       value => value.HasValue ? StrongID<UserTag>.From(value.Value) : null)
                    .IsRequired(false);

            builder.Property(x => x.DisabledAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //       Availability
            // -------------------------

            builder.Property(x => x.IsAvailable)
                .IsRequired();

            builder.Property(x => x.ActiveOrdersCount)
                .IsRequired();

            // -------------------------
            //          Active
            // -------------------------

            builder.Property(x => x.LastSeenAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //         Location
            // -------------------------

            builder.OwnsOne(x => x.CurrentLocation, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("CurrentLatitude")
                    .HasPrecision(9, 6)
                    .IsRequired(false);

                location.Property(p => p.Longitude)
                    .HasColumnName("CurrentLongitude")
                    .HasPrecision(9, 6)
                    .IsRequired(false);
            });

            builder.Property(x => x.LastLocationAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //          Rating
            // -------------------------

            builder.Property(x => x.AverageRating)
                .HasPrecision(3, 2)
                .IsRequired();

            builder.Property(x => x.RatingsCount)
                .IsRequired();

            // -------------------------
            //         Approval
            // -------------------------

            builder.Property(x => x.ApprovedByAdminID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.Property(x => x.ApprovedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            // Driver = User (1:1)
            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Driver>(x => x.ID)
                .OnDelete(DeleteBehavior.Restrict);

            // ApprovedByAdmin
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.ApprovedByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // DisabledByAdmin
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.DisabledByAdminID)
                .OnDelete(DeleteBehavior.Restrict);
            // VehicleType
            builder.HasOne<VehicleType>()
                .WithMany()
                .HasForeignKey(x => x.VehicleTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Indexes
            // -------------------------
            //انا شايفو مو ضروري 
            builder.HasIndex(x => x.VehicleTypeID);
        }
    }
}

