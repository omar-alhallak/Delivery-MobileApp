using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.MerchantsConfiguration.CatalogConfiguration
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<ProductTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.MerchantCategoryID) // One(MerchantCategory) -----> Many(Product) || التصنيف الخاص بالمنتج
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantCategoryTag>.From(value))
                .IsRequired();

            builder.HasOne<MerchantCategory>()
                .WithMany()
                .HasForeignKey(x => x.MerchantCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Basic Info --------

            builder.Property(x => x.ProductName)
                .HasConversion(
                    value => value.Value,
                    value => DisplayName.Create(value, 150, nameof(Product.ProductName)))
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.BasePrice)
                .HasPrecision(18, 2)
                .IsRequired(false);
                 
            builder.Property(x => x.SortOrder)
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

            builder.HasIndex(x => x.MerchantCategoryID); // جلب جميع منتجات تصنيف معين

            builder.HasIndex(x => new // منع تكرار اسم المنتج داخل نفس التصنيف
            {
                x.MerchantCategoryID,
                x.ProductName
            })     .IsUnique();

            builder.HasIndex(x => new // منع تكرار نفس الترتيب داخل نفس التصنيف
            {
                x.MerchantCategoryID,
                x.SortOrder
            })     .IsUnique();

            builder.HasIndex(x => new // جلب المنتجات الفعالة داخل التصنيف مرتبة
            {
                x.MerchantCategoryID,
                x.IsActive,
                x.SortOrder
            })     .HasFilter("[IsActive] = 1");
        }
    }
}