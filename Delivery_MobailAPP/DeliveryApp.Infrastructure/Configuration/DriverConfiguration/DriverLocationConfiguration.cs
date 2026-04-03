using DeliveryApp.Domain.Entities.Drivers;
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
    public sealed class DriverLocationConfiguration : IEntityTypeConfiguration<DriverLocation>
    {
        public void Configure(EntityTypeBuilder<DriverLocation> builder)
        {
            builder.ToTable("DriverLocations");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<DriverLocationTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //         Relations
            // -------------------------

            builder.Property(x => x.DriverID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            // -------------------------
            //          Location
            // -------------------------

            builder.OwnsOne(x => x.Location, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("Latitude")
                    .HasPrecision(9, 6)
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("Longitude")
                    .HasPrecision(9, 6)
                    .IsRequired();
            });

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.RecordedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<Driver>()
                .WithMany()
                .HasForeignKey(x => x.DriverID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Indexes
            // -------------------------
            //هاد خايف منو 
            builder.HasIndex(x => new { x.DriverID, x.RecordedAt });
        }
    }
}
