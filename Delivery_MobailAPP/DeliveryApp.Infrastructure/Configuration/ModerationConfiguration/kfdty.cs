using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration
{
    public sealed class kfdty : IEntityTypeConfiguration<AccountWarning>
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
                .ValueGeneratedNever()
                .IsRequired();

            // -------------------------
            //      ForeignKey
            // -------------------------

            builder.Property(x => x.RelatedOrderID)
                .HasConversion(
                    id => id!.Value.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.RelatedOrderID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            // -----------------------------

            builder.Property(x => x.EntityID)
               .HasConversion(
                    id => id,
                    value => value
                )
              .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.EntityID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            // ----------------------------

            builder.Property(x => x.CreatedByAdminID)
              .HasConversion(
                  id => id.Value,
                  value => StrongID<UserTag>.From(value))
              .IsRequired();

            builder.HasOne<User>()
                 .WithMany()
                 .HasForeignKey(x => x.CreatedByAdminID)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);
            // -----------------------------
            builder.Property(x => x.DecidedByAdminID)
              .HasConversion(
                  id => id!.Value.Value,
                  value => StrongID<UserTag>.From(value))
              .IsRequired();

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.DecidedByAdminID)
               .IsRequired()
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
            //       restrictions
            // -------------------------

            builder.Property(x => x.ReasonDetails)
                .HasMaxLength(1000)
                .IsUnicode(true);

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

            builder.HasIndex(x => new { x.EntityID, x.EntityType })
                .HasDatabaseName("IX_AccountWarning_Entity_Type").IsUnique();
            
            builder.HasIndex(x => x.RelatedOrderID)
               .HasDatabaseName("IX_AccountWarning_RelatedOrder_Optional")
               .HasFilter("[RelatedOrderID] IS NOT NULL");
        }

    }
}
