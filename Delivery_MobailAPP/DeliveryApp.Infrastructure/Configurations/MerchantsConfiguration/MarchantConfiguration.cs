using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.MerchantsConfiguration
{
    public sealed class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("Merchants", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
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
            //            Enums
            // -------------------------

            builder.Property(x => x.MerchantType)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Basic Info --------

            builder.Property(x => x.MerchantName)
                .HasConversion(
                    value => value.Value,
                    value => DisplayName.Create(value, 150, nameof(Merchant.MerchantName)))
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.Slug)
                .HasConversion(
                    value => value.Value,
                    value => Slug.Create(value))
                .HasMaxLength(80)
                .IsUnicode(false)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.LogoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.CoverImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.DefaultPreparationTime)
                 .HasColumnType("time")
                .IsRequired();

            // -------- Location --------

            builder.OwnsOne(x => x.Location, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("Latitude")
                    .HasColumnType("float")
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("Longitude")
                    .HasColumnType("float")
                    .IsRequired();
            });

            // -------- Rating --------

            builder.Property(x => x.AverageRating)
                .HasPrecision(3, 2)
                .IsRequired();

            builder.Property(x => x.RatingsCount)
                .IsRequired();

            // -------- State --------

            builder.Property(x => x.IsActive)
                .IsRequired();

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.PublicID)
                .IsUnique()
                .HasFilter("[PublicID] IS NOT NULL");

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => new
            {
                x.MerchantType,
                x.IsActive
            }).HasFilter("[IsActive] = 1");
        }
    }
}