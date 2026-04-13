using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration
{
    public sealed class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.ToTable("Complaint", "moderation");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<ComplaintTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.OrderID) // One(Order) -----> Many(Complaint) || الطلب المرتبط بالشكوى
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.CreatedByUserID) // One(User) -----> Many(Complaint) || المستخدم الي أنشأ الشكوى
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.ReviewedByAdminID) // One(User) -----> Many(Complaint) || المشرف الي راجع الشكوى
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<UserTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.ReviewedByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Target --------

            builder.Property(x => x.TargetType)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.TargetID)
                .IsRequired();

            // -------- Complaint Details --------

            builder.Property(x => x.Reason)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.Message)
                .HasMaxLength(1000)
                .IsUnicode(true)
                .IsRequired();

            // -------- Review --------

            builder.Property(x => x.Status)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.AdminResponse)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.ResolvedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.OrderID); // جلب جميع الشكاوى المرتبطة بطلب معين

            builder.HasIndex(x => x.Status); // جلب الشكاوى حسب حالتها

            builder.HasIndex(x => new // جلب الشكاوى الخاصة بجهة معينة مرتبة حسب وقت الإنشاء
            {
                x.TargetType,
                x.TargetID,
                x.CreatedAt
            });

            builder.HasIndex(x => new // منع تكرار الشكاوي المعلقة
            {
                x.CreatedByUserID,
                x.OrderID,
                x.TargetType,
                x.TargetID,
                x.Reason
            })  .IsUnique()
                .HasFilter("[Status] = 1");
        }
    }
}