using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.CustomersConfiguration
{
    public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses", "customers");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<AddressTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.UserID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //            Enums
            // -------------------------

            builder.Property(x => x.AddressType)
                .HasConversion<byte>()
                .IsRequired(false);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Basic Info --------

            builder.Property(x => x.Label)
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired(false);

            // -------- Location --------

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

            // -------- Details --------

            builder.Property(x => x.BuildingName)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.Floor)
                .HasMaxLength(50)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.DoorInfo)
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.Notes)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------- Flags --------

            builder.Property(x => x.IsDefault)
                .IsRequired();

            builder.Property(x => x.IsTemporary)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.UserID);

            builder.HasIndex(x => new 
            { 
                x.UserID,
                x.IsDefault
            })  .IsUnique()
                .HasFilter("[IsDefault] = 1");
        }
    }
}