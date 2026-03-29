using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeliveryApp.Infrastructure.Configuration.ModerationConfiguration
{
    public sealed class AccountWarningConfiguration : IEntityTypeConfiguration<AccountWarning>
    {
        public void Configure(EntityTypeBuilder<AccountWarning> builder)
        {
            builder.ToTable("AccountWarnings");

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
            //    Personal Information
            // -------------------------

            builder.Property(x => x.ReasonDetails)
                .HasMaxLength(1000)   
                .IsUnicode(true)      
                .IsRequired();

            // -------------------------
            //      ForeignKey
            // -------------------------

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.RelatedOrderID)
                .IsRequired(false)           
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.EntityID) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByAdminID) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.DecidedByAdminID) 
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //      Target Entity
            // -------------------------

            builder.Property(x => x.EntityType)
                .HasConversion<int>() 
                .IsRequired();

            // -------------------------
            //      Warning Details
            // -------------------------

            builder.Property(x => x.Reason)
                .HasConversion<int>() 
                .IsRequired();

            builder.Property(x => x.Severity)
                .HasConversion<int>() 
                .IsRequired();


            // -------------------------
            //         Decision
            // -------------------------

            builder.Property(x => x.Decision)
                .HasConversion<int>() 
                .IsRequired();

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset") 
                .IsRequired(); 

            builder.Property(x => x.DecidedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false); 

            builder.Property(x => x.ExpiresAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.RelatedOrderID);
            builder.HasIndex(x => new { x.EntityType, x.EntityID });
        }

    }
}
