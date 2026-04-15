using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation;
using DeliveryApp.Domain.Entities.Customers.Orders;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.ModerationConfiguration
{
    public sealed class AccountWarningConfiguration : IEntityTypeConfiguration<AccountWarning>
    {
        public void Configure(EntityTypeBuilder<AccountWarning> builder)
        {
            builder.ToTable("AccountWarning", "moderation");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<AccountWarningTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.RelatedOrderID) // One(Order) -----> Many(AccountWarning) || الطلب المرتبط بالتحذير
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<OrderTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.RelatedOrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.CreatedByAdminID) // One(User) -----> Many(AccountWarning) || المشرف الي أنشأ التحذير
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.DecidedByAdminID) // One(User) -----> Many(AccountWarning) || المشرف الي حسم القرار
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<UserTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.DecidedByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //            Enums
            // -------------------------

            builder.Property(x => x.EntityType)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.Severity)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.Decision)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Target Entity --------

            builder.Property(x => x.EntityID)
                .IsRequired();

            // -------- Warning Details --------

            builder.Property(x => x.ReasonDetails)
                .HasMaxLength(1000)
                .IsUnicode(true)
                .IsRequired();

            // -------- Status --------

            builder.Property(x => x.IsActive)
                .IsRequired();

            // -------- Expiry --------

            builder.Property(x => x.ExpiresAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.DecidedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.RelatedOrderID) // جلب التحذيرات المرتبطة بطلب معين
                .HasFilter("[RelatedOrderID] IS NOT NULL");

            builder.HasIndex(x => x.Decision); // جلب التحذيرات حسب الحالة

            builder.HasIndex(x => new // جلب التحذيرات الخاصة بكيان معين مرتبة حسب وقت الإنشاء
            {
                x.EntityType,
                x.EntityID,
                x.CreatedAt
            });

            builder.HasIndex(x => new // منع تكرار تحذير غير محسوم
            {       
                x.EntityType,
                x.EntityID,
                x.RelatedOrderID,
                x.Reason
            })     .IsUnique()
                   .HasFilter("[IsActive] = 1");
        }
    }
}