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
    public sealed class VichicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
    {
        public void Configure(EntityTypeBuilder<VehicleType> builder) 
        {
            builder.ToTable("VehicleTypes");

            // Key
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<VehicleTypeTag>.From(value))
                .ValueGeneratedNever();

            // Name
            builder.Property(x => x.VehicleName)
                .HasMaxLength(100)
                .IsRequired();

            // Limits
            builder.Property(x => x.MaxDistanceKm).IsRequired();
            builder.Property(x => x.MaxMergeExtraKm).IsRequired();
            builder.Property(x => x.MaxOrdersToBatch).IsRequired();

            // Commission
            builder.Property(x => x.CommissionPercent).IsRequired();

            // Flags
            builder.Property(x => x.RequiresLicenseAndPlate).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            // Index 
            builder.HasIndex(x => x.VehicleName).IsUnique();
        }

    }
}
