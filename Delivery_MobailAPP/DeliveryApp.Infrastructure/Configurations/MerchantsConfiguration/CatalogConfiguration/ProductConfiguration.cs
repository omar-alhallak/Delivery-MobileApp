using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.Merchants.Catalog
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "Merchants");

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
            //       Relationships
            // -------------------------

            builder.Property(x => x.MerchantCategoryID)
               .HasConversion(
                   id => id.Value,
                   value => StrongID<MerchantCategoryTag>.From(value))
               .IsRequired();

            builder.HasOne<MerchantCategory>()
                .WithMany()
                .HasForeignKey(x => x.MerchantCategoryID)
                .OnDelete(DeleteBehavior.Restrict);


            // -------------------------
            //        Basic Info
            // -------------------------

            builder.Property(x => x.ProductName)
                .HasConversion(
                    value => value.Value,
                    value => CatalogName.Create(value, 150, nameof(Product.ProductName)))
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

            // -------------------------
            //          State
            // -------------------------

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

            builder.HasIndex(x => x.MerchantCategoryID);

            builder.HasIndex(x => new { x.MerchantCategoryID, x.ProductName })
                .IsUnique();
        }
    }
}