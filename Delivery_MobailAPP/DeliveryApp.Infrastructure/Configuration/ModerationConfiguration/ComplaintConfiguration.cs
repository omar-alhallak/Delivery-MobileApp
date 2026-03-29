using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.ValueObjects; 
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DeliveryApp.Domain.Entities.Orders;

namespace DeliveryApp.Infrastructure.Configuration.ModerationConfiguration
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
                .ValueGeneratedNever();

            // -------------------------
            //         Target
            // -------------------------

            builder.Property(x => x.TargetType)
                .HasConversion<int>()
                .IsRequired();

            // -------------------------
            //      Complaint Details
            // -------------------------

            builder.Property(x => x.Reason)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Message)
                .HasMaxLength(2000)
                .IsUnicode(true)
                .IsRequired();

            // -------------------------
            //          Review
            // -------------------------

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired(); 

            builder.Property(x => x.AdminResponse)
                .HasMaxLength(2000)
                .IsUnicode(true)
                .IsRequired(false);

            // -------------------------
            //      ForeignKey
            // -------------------------

            builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(x => x.CreatedByUserID) 
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
             .WithMany()
             .HasForeignKey(x => x.ReviewedByAdminID) 
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Order>()
             .WithMany()
             .HasForeignKey(x => x.OrderID) 
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.TargetID) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

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

            builder.HasIndex(x => x.OrderID); 
            builder.HasIndex(x => new { x.TargetType, x.TargetID }); 
        }
    }
    
}
