using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.MerchantsConfiguration
{
    public sealed class MerchantWorkingHourConfiguration : IEntityTypeConfiguration<MerchantWorkingHour>
    {
        public void Configure(EntityTypeBuilder<MerchantWorkingHour> builder)
        {
            builder.ToTable("MerchantWorkingHour", "merchants");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantWorkingHourTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.MerchantID) // One(Merchant) -----> Many(MerchantWorkingHour) || المطعم الي له السجل
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .IsRequired();

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Enums
            // -------------------------

            builder.Property(x => x.Day)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Working Hours --------

            builder.Property(x => x.OpenTime)
                .HasColumnType("time")
                .IsRequired(false);

            builder.Property(x => x.CloseTime)
                .HasColumnType("time")
                .IsRequired(false);

            builder.Property(x => x.IsClosed)
                .IsRequired();

            builder.Property(x => x.IsOpenAllDay)
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.MerchantID); // جلب جميع أوقات العمل للتاجر

            builder.HasIndex(x => new { x.MerchantID, x.Day }) // منع تكرار نفس اليوم لنفس التاجر
                .IsUnique();
        }
    }
}