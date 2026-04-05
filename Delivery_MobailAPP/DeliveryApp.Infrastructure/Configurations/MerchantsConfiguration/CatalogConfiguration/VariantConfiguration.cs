using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.Merchants.Catalog
{
    public sealed class VariantConfiguration : IEntityTypeConfiguration<Variant>
    {
        public void Configure(EntityTypeBuilder<Variant> builder)
        {
            builder.ToTable("Variants", "Merchants");

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
            //         Relations
            // -------------------------

            builder.Property(x => x.ProductID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<ProductTag>.From(value))
                .IsRequired();

            // -------------------------
            //        Basic Info
            // -------------------------

            builder.Property(x => x.VariantName)
                .HasConversion(
                    value => value.Value,
                    value => CatalogName.Create(value, 100, nameof(Variant.VariantName)))
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.BasePrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // -------------------------
            //          State
            // -------------------------

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(x => x.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.ProductID);

            builder.HasIndex(x => new { x.ProductID, x.VariantName })
                .IsUnique();
        }
    }
}