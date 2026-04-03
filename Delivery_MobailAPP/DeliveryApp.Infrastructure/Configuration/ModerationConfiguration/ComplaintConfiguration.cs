using DeliveryApp.Domain.ValueObjects; 
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DeliveryApp.Domain.Entities.Orders;

namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration
{
    public sealed class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.ToTable("Complaints");
            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<ComplaintTag>.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            // -------------------------
            //      ForeignKey
            // -------------------------
            builder.Property(x => x.OrderID)
               .HasConversion(
                   id => id!.Value,
                   value => StrongID<OrderTag>.From(value))
               .IsRequired();

            builder.HasOne<Order>()
               .WithMany()
               .HasForeignKey(x => x.OrderID)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
            // ------------------------------

            builder.Property(x => x.TargetID)
               .HasConversion(
                    id => id,
                    value => value
                )
              .IsRequired(false);

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.TargetID)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
            // -------------------------------

            builder.Property(x => x.CreatedByUserID)
              .HasConversion(
                  id => id.Value,
                  value => StrongID<UserTag>.From(value))
              .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            // -------------------------------

            builder.Property(x => x.ReviewedByAdminID)
              .HasConversion(
                  id => id!.Value.Value,
                  value => StrongID<UserTag>.From(value))
              .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.ReviewedByAdminID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //         Target
            // -------------------------

            builder.Property(x => x.TargetType)
                .HasConversion<int>()
                .IsRequired();

            // -------------------------
            //          Enums
            // -------------------------

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasConversion<int>()
                .IsRequired();

            // -------------------------
            //      Complaint Details
            // -------------------------

            builder.Property(x => x.Message)
                .HasMaxLength(1000)
                .IsUnicode(true)
                .IsRequired();

            // -------------------------
            //          Review
            // -------------------------

            builder.Property(x => x.AdminResponse)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired();

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.ResolvedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new { x.TargetType, x.TargetID })
                .HasDatabaseName("IX_Complaints_Target");

            builder.HasIndex(x => x.CreatedByUserID)
                .HasDatabaseName("IX_Complaint_CreatorUser");

        }
    }
    
}
