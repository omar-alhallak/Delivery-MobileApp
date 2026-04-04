using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.CustomersConfiguration.OrderConfiguration
{
    public sealed class OrderAssignmentConfiguration : IEntityTypeConfiguration<OrderAssignment>
    {
        public void Configure(EntityTypeBuilder<OrderAssignment> builder)
        {
            builder.ToTable("OrderAssignments", "Customers");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .ValueGeneratedNever();

            // -------------------------
            //         Relations
            // -------------------------

            builder.Property(x => x.OrderID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            builder.Property(x => x.DriverID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            // -------------------------
            //          Status
            // -------------------------

            builder.Property(x => x.Status)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //          Remove
            // -------------------------

            builder.Property(x => x.RemovedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            builder.Property(x => x.RemoveReason)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.AssignedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.DriverID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.OrderID);

            builder.HasIndex(x => x.DriverID);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => new { x.OrderID, x.Status });
        }
    }
}