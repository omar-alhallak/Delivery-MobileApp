using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.MerchantsConfiguration.CatalogConfiguration
{
    public sealed class VariantConfiguration : IEntityTypeConfiguration<Variant>
    {
        public void Configure(EntityTypeBuilder<Variant> builder)
        {
            builder.ToTable("Variant", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<VariantTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.ProductID) // One(Product) -----> Many(Variant) || المنتج الي يتبع له ال Variant
                .HasConversion(
                    id => id.Value,
                    value => StrongID<ProductTag>.From(value))
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(x => x.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Basic Info --------

            builder.Property(x => x.VariantName)
                .HasConversion(
                    value => value.Value,
                    value => DisplayName.Create(value, 100, nameof(Variant.VariantName)))
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.BasePrice)
                .HasPrecision(18, 2)
                .IsRequired();

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

            builder.HasIndex(x => x.ProductID); // جلب جميع التفصيلات الخاصة بمنتج معين

            builder.HasIndex(x => new // منع تكرار اسم التفصيل داخل نفس المنتج
            {
                x.ProductID,
                x.VariantName
            })     .IsUnique();

            builder.HasIndex(x => new // منع تكرار نفس الترتيب داخل نفس المنتج
            {
                x.ProductID,
                x.SortOrder
            })     .IsUnique();

            builder.HasIndex(x => new // جلب التفصيل الفعالة لمنتج معين مرتبة
            {
                x.ProductID,
                x.IsActive,
                x.SortOrder
            })     .HasFilter("[IsActive] = 1");
        }
    }
}