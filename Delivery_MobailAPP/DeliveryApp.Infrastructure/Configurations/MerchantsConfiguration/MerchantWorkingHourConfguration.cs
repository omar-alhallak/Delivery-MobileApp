

using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.MerchantConfiguration
{
    public sealed class MerchantWorkingHourConfiguration : IEntityTypeConfiguration<MerchantWorkingHour>
    {
        public void Configure(EntityTypeBuilder<MerchantWorkingHour> builder)
        {
            builder.ToTable("MerchantWorkingHours", "merchant");

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
            //      Relation Keys
            // -------------------------

            builder.Property(x => x.MerchantID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<MerchantTag>.From(value))
                .IsRequired();

            // -------------------------
            //        Working Day
            // -------------------------

            builder.Property(x => x.Day)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //      Working Hours
            // -------------------------

            builder.Property(x => x.OpenTime)
                .HasColumnType("time")
                .IsRequired(false);

            builder.Property(x => x.CloseTime)
                .HasColumnType("time")
                .IsRequired(false);

            builder.Property(x => x.IsClosed)
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new { x.MerchantID, x.Day })
                .IsUnique();
        }
    }
}