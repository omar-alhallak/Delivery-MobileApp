using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities.Drivers;
using DeliveryApp.Domain.Entities.Identity; 
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.DriverConfiguration
{
    public sealed class DriverRequestConfiguration : IEntityTypeConfiguration<DriverRequest>
    {
        public void Configure(EntityTypeBuilder<DriverRequest> builder)
        {
            builder.ToTable("DriverRequests");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<DriverRequestTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //         Relations
            // -------------------------

            builder.Property(x => x.UserID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.Property(x => x.VehicleTypeID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<VehicleTypeTag>.From(value))
                .IsRequired();

            // -------------------------
            //      Personal Info
            // -------------------------

            builder.Property(x => x.FullName)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.FatherName)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.NationalIdNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired();

            // -------------------------
            //          Photos
            // -------------------------

            builder.Property(x => x.PersonalPhotoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired();

            builder.Property(x => x.NationalIdPhotoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired();

            // -------------------------
            //     Vehicle Details
            // -------------------------

            builder.Property(x => x.DrivingLicensePhotoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.DrivingLicenseNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.VehiclePlateNumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsRequired(false);

            // -------------------------
            //          Review
            // -------------------------

            builder.Property(x => x.Status)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.ReviewedByAdminID)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<UserTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.Property(x => x.ReviewedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            builder.Property(x => x.RejectionReason)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<VehicleType>()
                .WithMany()
                .HasForeignKey(x => x.VehicleTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.ReviewedByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Indexes
            // -------------------------
            // هاد ويلي تحتو مهمين 
            builder.HasIndex(x => x.UserID)
                .IsUnique()
                .HasFilter("[Status] = 1");

            builder.HasIndex(x => x.NationalIdNumber)
                .IsUnique()
                .HasFilter("[Status] IN (1, 2)");
            //يمكن مو ضروري شييك عليه
            builder.HasIndex(x => x.VehicleTypeID);
            // هاد اذا بدنا نستعلم حسب حالة الطلب 
            builder.HasIndex(x => x.Status);
        }
    }
}
