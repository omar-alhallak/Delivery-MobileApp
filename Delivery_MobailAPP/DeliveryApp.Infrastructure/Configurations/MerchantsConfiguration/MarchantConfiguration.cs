using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.MerchantsConfiguration
{
    public sealed class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("Merchants", "Merchant");

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
            //           Enums
            // -------------------------

            builder.Property(x =>x.MerchantType)
                .HasConversion<int>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // ----------- Basic Info ----------

            builder.OwnsOne(x => x.MerchantName, name =>
            {
                name.Property(p => p.Value)
                    .HasColumnName("MerchantName") 
                    .HasMaxLength(150)             
                    .IsRequired()
                    .IsUnicode(true);              
            });

            builder.OwnsOne(x => x.Slug, slug =>
            {
                slug.Property(s => s.Value)
                    .HasColumnName("Slug")      
                    .HasMaxLength(80)          
                    .IsRequired()
                    .IsUnicode(false);          
            });

            builder.Property(x => x.Description)
                .HasMaxLength(2000)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x=>x.LogoUrl)
                .HasMaxLength(500)
                .IsUnicode (true)
                .IsRequired(false);

            builder.Property(x=>x.CoverImageUrl)
                .HasMaxLength (500)
                .IsUnicode(true)
                .IsRequired(false);

            // ------------- Location -----------

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

            // ------------ Rating --------------

            builder.Property(x => x.AverageRating)
                .HasPrecision(3, 2)
                .IsRequired();

            builder.Property(x => x.RatingsCount)
                .IsRequired();

            // ----------- State ----------------

            builder.Property(x=>x.IsActive)
                .IsRequired();

            // ----------- Dates ----------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.PublicID)
                .IsUnique()
                .HasFilter("[PublicID] IS NOT NULL");

            builder.HasIndex(x => new { x.MerchantType, x.IsActive })
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("IX_Merchant_Type_Active");

            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName("UX_Merchant_Slug");

        }
    }
}
