using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Drivers;
using DeliveryApp.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.DriversConfiguration
{
    public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Driver", "drivers");

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
            //      PK Relationship
            // -------------------------

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Driver>(x => x.ID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.VehicleTypeID) // One(VehicleType) -----> Many(Driver) || نوع المركبة السائق
                .HasConversion(
                    id => id.Value,
                    value => StrongID<VehicleTypeTag>.From(value))
                .IsRequired();

            builder.HasOne<VehicleType>()
                .WithMany()
                .HasForeignKey(x => x.VehicleTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.DisabledByAdminID) // One(User) -----> Many(Driver) || المشرف الي عطل السائق
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<UserTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.DisabledByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.ApprovedByAdminID) // One(User) -----> Many(Driver) || المشرف الي وافق على السائق
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.ApprovedByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Admin Control --------

            builder.Property(x => x.IsEnabled)
                .IsRequired();

            builder.Property(x => x.DisabledAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------- Availability --------

            builder.Property(x => x.IsAvailable)
                .IsRequired();

            builder.Property(x => x.ActiveOrdersCount)
                .IsRequired();

            // -------- Active --------

            builder.Property(x => x.LastSeenAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------- Location --------

            builder.OwnsOne(x => x.CurrentLocation, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("CurrentLatitude")
                    .HasColumnType("float");

                location.Property(p => p.Longitude)
                    .HasColumnName("CurrentLongitude")
                    .HasColumnType("float");
            });

            builder.Property(x => x.LastLocationAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------- Rating --------

            builder.Property(x => x.AverageRating)
                .HasPrecision(3, 2)
                .IsRequired();

            builder.Property(x => x.RatingsCount)
                .IsRequired();

            // -------- Approval --------

            builder.Property(x => x.ApprovedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new // جلب السائقين حسب نوع المركبة والتفعيل والتوفر وآخر نشاط
            {
                x.VehicleTypeID,
                x.IsEnabled,
                x.IsAvailable,
                x.LastSeenAt
            });
        }
    }
}