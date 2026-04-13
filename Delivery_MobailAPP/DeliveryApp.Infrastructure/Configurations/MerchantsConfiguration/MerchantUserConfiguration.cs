using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.MerchantsConfiguration
{
    public sealed class MerchantUserConfiguration : IEntityTypeConfiguration<MerchantUser>
    {
        public void Configure(EntityTypeBuilder<MerchantUser> builder)
        {
            builder.ToTable("MerchantUser", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => new { x.MerchantID, x.UserID });

            // -------------------------
            //        Foreign Keys
            // -------------------------

            //                           جدول كسر 
            // One(Merchant) -----> Many(MerchantUser) <----- One(User)

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

            builder.Property(x => x.UserID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //            Enums
            // -------------------------

            builder.Property(x => x.Role)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

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

            builder.HasIndex(x => x.UserID); // جلب جميع المطاعم المرتبط بالمستخدم
        }
    }
}