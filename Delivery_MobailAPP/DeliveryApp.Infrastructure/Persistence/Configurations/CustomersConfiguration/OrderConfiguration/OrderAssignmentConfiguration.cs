using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.CustomersConfiguration.OrderConfiguration
{
    public sealed class OrderAssignmentConfiguration : IEntityTypeConfiguration<OrderAssignment>
    {
        public void Configure(EntityTypeBuilder<OrderAssignment> builder)
        {
            builder.ToTable("OrderAssignment", "customers");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.OrderID) // One(Order) -----> Many(OrderAssignment) || لأي طلب تابع هاد الإسناد
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.DriverID) // One(User) -----> Many(OrderAssignment) || لأي سائق تابع هاد الإسناد
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.DriverID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //            Enums
            // -------------------------

            builder.Property(x => x.Status)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Dates --------

            builder.Property(x => x.AssignedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------- Remove --------

            builder.Property(x => x.RemovedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            builder.Property(x => x.RemoveReason)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new // منع إضافة نفس السائق أكتر من مرة لنفس الطلب
            {           
                x.OrderID,
                x.DriverID
            })     .IsUnique();

            builder.HasIndex(x => new // جلب الطلبات المسندة إلا سائق معين حسب الحالة
            {
                x.DriverID,
                x.Status
            });

            builder.HasIndex(x => new // جلب السائقين المسند إليهم طلب معين حسب الحالة
            {
                x.OrderID,
                x.Status
            });
        }
    }
}