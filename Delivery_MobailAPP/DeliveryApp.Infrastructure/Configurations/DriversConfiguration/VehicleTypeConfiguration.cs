using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Drivers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.DriversConfiguration
{
    public sealed class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
    {
        public void Configure(EntityTypeBuilder<VehicleType> builder)
        {
            builder.ToTable("VehicleType", "drivers");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<VehicleTypeTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Vehicle Info --------

            builder.Property(x => x.VehicleName)
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired();

            // -------- Delivery Limits --------

            builder.Property(x => x.MaxDistanceKm)
                .HasColumnType("float")
                .IsRequired();

            builder.Property(x => x.MaxMergeExtraKm)
                .HasColumnType("float")
                .IsRequired();

            builder.Property(x => x.MaxOrdersToBatch)
                .IsRequired();

            // -------- Commission --------

            builder.Property(x => x.CommissionPercent)
                .IsRequired();

            // -------- Requird --------

            builder.Property(x => x.RequiresLicenseAndPlate)
                .IsRequired();

            // -------- Status --------

            builder.Property(x => x.IsActive)
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.VehicleName) // منع تكرار اسم المركبة
                .IsUnique();
        }
    }
}