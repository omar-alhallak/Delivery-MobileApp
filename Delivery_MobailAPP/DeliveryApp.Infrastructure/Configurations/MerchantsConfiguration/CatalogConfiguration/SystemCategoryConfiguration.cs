using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.Merchants.Catalog
{
    public sealed class SystemCategoryConfiguration : IEntityTypeConfiguration<SystemCategory>
    {
        public void Configure(EntityTypeBuilder<SystemCategory> builder)
        {
            builder.ToTable("SystemCategories", "Merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<SystemCategoryTag>.From(value))
                .ValueGeneratedNever();

            builder.Property(x => x.MerchantType)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //        Basic Info
            // -------------------------

            builder.Property(x => x.CategoryName)
                .HasConversion(
                    value => value.Value,
                    value => CatalogName.Create(value, 150, nameof(SystemCategory.CategoryName)))
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

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            // -------------------------
            //         Display
            // -------------------------

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            // Slug unique داخل نفس نوع التاجر
            builder.HasIndex(x => new { x.MerchantType, x.Slug })
                .IsUnique();

            // منع تكرار الاسم داخل نفس النوع
            builder.HasIndex(x => new { x.MerchantType, x.CategoryName })
                .IsUnique();
        }
    }
}