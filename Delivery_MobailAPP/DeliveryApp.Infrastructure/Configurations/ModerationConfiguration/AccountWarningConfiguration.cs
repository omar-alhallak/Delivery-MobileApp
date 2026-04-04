using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration
{
    public sealed class AccountWarningConfiguration : IEntityTypeConfiguration<AccountWarning>
    {
        public void Configure(EntityTypeBuilder<AccountWarning> builder)
        {
            builder.ToTable("AccountWarnings", "moderation");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<AccountWarningTag>.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.RelatedOrderID)
                .HasConversion(
                    id => id.HasValue ? id.Value.Value : (Guid?)null,
                    value => value.HasValue ? StrongID<OrderTag>.From(value.Value) : null)
                .IsRequired(false);

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.RelatedOrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.CreatedByAdminID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByAdminID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.DecidedByAdminID)
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

            builder.HasIndex(x => x.RelatedOrderID)
                .HasFilter("[RelatedOrderID] IS NOT NULL");

            builder.HasIndex(x => x.Decision);

            builder.HasIndex(x => new 
            { 
                x.EntityType,
                x.EntityID 
            });
        }
    }
}