using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.Merchants.Catalog
{
    public sealed class MerchantCategoryConfiguration : IEntityTypeConfiguration<MerchantCategory>
    {
        public void Configure(EntityTypeBuilder<MerchantCategory> builder)
        {
            builder.ToTable("MerchantCategories", "Merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantCategoryTag>.From(value))
                .ValueGeneratedNever();

            builder.Property(x => x.MerchantID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            //       Basic Info
            // -------------------------

            builder.Property(x => x.CategoryName)
                .HasConversion(
                    value => value.Value,
                    value => CatalogName.Create(value, 150, nameof(MerchantCategory.CategoryName)))
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

            // -------------------------
            //         Display
            // -------------------------

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            // -------------------------
            //          Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.MerchantID);

            builder.HasIndex(x => new { x.MerchantID, x.CategoryName })
                .IsUnique();
        }
    }
}