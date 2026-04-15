using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.MerchantsConfiguration.CatalogConfiguration
{
    public sealed class MerchantCategoryConfiguration : IEntityTypeConfiguration<MerchantCategory>
    {
        public void Configure(EntityTypeBuilder<MerchantCategory> builder)
        {
            builder.ToTable("MerchantCategory", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantCategoryTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.MerchantID) // One(Merchant) -----> Many(MerchantCategory) || تصنيف تابع لأي مطعم
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .IsRequired();

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Basic Info --------

            builder.Property(x => x.CategoryName)
                .HasConversion(
                    value => value.Value,
                    value => DisplayName.Create(value, 150, nameof(MerchantCategory.CategoryName)))
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            // -------- Display --------

            builder.Property(x => x.SortOrder)
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

            builder.HasIndex(x => x.MerchantID); // جلب جميع تصنيفات مطعم معين

            builder.HasIndex(x => new // منع تكرار اسم التصنيف داخل نفس المطعم
            {           
                x.MerchantID,
                x.CategoryName
            })     .IsUnique();

            builder.HasIndex(x => new // منع تكرار نفس الترتيب داخل نفس المطعم
            {
                x.MerchantID,
                x.SortOrder
            })     .IsUnique();

            builder.HasIndex(x => new // جلب التصنيفات الفعالة لمطعم معين مرتبة
            {           
                x.MerchantID,
                x.IsActive,
                x.SortOrder
            })     .HasFilter("[IsActive] = 1");
        }
    }
}