using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.Merchants.Catalog
{
    public sealed class SystemCategoryConfiguration : IEntityTypeConfiguration<SystemCategory>
    {
        public void Configure(EntityTypeBuilder<SystemCategory> builder)
        {
            builder.ToTable("SystemCategory", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<SystemCategoryTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //           Enums
            // -------------------------

            builder.Property(x => x.MerchantType)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Basic Info --------

            builder.Property(x => x.CategoryName)
                .HasConversion(
                    value => value.Value,
                    value => DisplayName.Create(value, 150, nameof(SystemCategory.CategoryName)))
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

            builder.HasIndex(x => new // منع تكرار 
            {                         // slug داخل نفس نوع التاجر
                x.MerchantType,
                x.Slug
            })     .IsUnique();

            builder.HasIndex(x => new // منع تكرار التصنيف للتاجر
            {
                x.MerchantType,
                x.CategoryName
            })     .IsUnique();

            builder.HasIndex(x => new // جلب التصنيفات الفعالة مرتبة
            {
                x.MerchantType,
                x.IsActive,
                x.SortOrder
            })     .HasFilter("[IsActive] = 1");
        }
    }
}