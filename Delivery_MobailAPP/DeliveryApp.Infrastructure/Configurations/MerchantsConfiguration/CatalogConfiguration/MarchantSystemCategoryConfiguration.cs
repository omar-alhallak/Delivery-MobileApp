using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.Merchants.Catalog
{
    public sealed class MerchantSystemCategoryConfiguration : IEntityTypeConfiguration<MerchantSystemCategory>
    {
        public void Configure(EntityTypeBuilder<MerchantSystemCategory> builder)
        {
            builder.ToTable("MerchantSystemCategories","Merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => new { x.MerchantID, x.SystemCategoryID });

            // -------------------------
            //         Relations
            // -------------------------

            builder.Property(x => x.MerchantID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .IsRequired();

            builder.Property(x => x.SystemCategoryID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<SystemCategoryTag>.From(value))
                .IsRequired();

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<SystemCategory>()
                .WithMany()
                .HasForeignKey(x => x.SystemCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.SystemCategoryID);
        }
    }
}