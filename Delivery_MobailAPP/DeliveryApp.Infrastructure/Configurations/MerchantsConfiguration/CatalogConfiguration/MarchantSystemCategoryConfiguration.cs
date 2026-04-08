using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.Merchants.Catalog
{
    public sealed class MerchantSystemCategoryConfiguration : IEntityTypeConfiguration<MerchantSystemCategory>
    {
        public void Configure(EntityTypeBuilder<MerchantSystemCategory> builder)
        {
            builder.ToTable("MerchantSystemCategories", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => new { x.MerchantID, x.SystemCategoryID });

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.MerchantID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .ValueGeneratedNever();

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------

            builder.Property(x => x.SystemCategoryID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<SystemCategoryTag>.From(value))
                .ValueGeneratedNever();

            builder.HasOne<SystemCategory>()
                .WithMany()
                .HasForeignKey(x => x.SystemCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.SystemCategoryID);
        }
    }
}