using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.MerchantsConfiguration.CatalogConfiguration
{
    public sealed class MerchantSystemCategoryConfiguration : IEntityTypeConfiguration<MerchantSystemCategory>
    {
        public void Configure(EntityTypeBuilder<MerchantSystemCategory> builder)
        {
            builder.ToTable("MerchantSystemCategory", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => new { x.MerchantID, x.SystemCategoryID });

            // -------------------------
            //        Foreign Keys
            // -------------------------
            //                               جدول كسر 
            // One(Merchant) -----> Many(MerchantSystemCategory) <----- One(SystemCategory)

            builder.Property(x => x.MerchantID) 
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .IsRequired();

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.SystemCategoryID) 
                .HasConversion(
                    id => id.Value,
                    value => StrongID<SystemCategoryTag>.From(value))
                .IsRequired();

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

            builder.HasIndex(x => x.SystemCategoryID); // جلب المطاعم المرتبطة بتصنيف معين
        }
    }
}