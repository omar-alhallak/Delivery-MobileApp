using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.MerchantsConfiguration
{
    public sealed class MerchantUserConfiguration : IEntityTypeConfiguration<MerchantUser>
    {
        public void Configure(EntityTypeBuilder<MerchantUser> builder)
        {
            builder.ToTable("MerchantUsers","merchant");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => new { x.MerchantID, x.UserID });

            builder.Property(x => x.MerchantID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .ValueGeneratedNever();

            builder.Property(x => x.UserID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //           Enums
            // -------------------------

            builder.Property(x => x.Role)
                .HasConversion<int>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // ----------- State ----------------

            builder.Property(x => x.IsActive)
                .IsRequired();

            // ----------- Dates ----------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new { x.UserID, x.MerchantID, x.Role })
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("IX_MerchantUser_ActiveAccess");
        }
    }
}
