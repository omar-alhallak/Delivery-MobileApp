using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Engagements;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.EngagementsConfiguration
{
    public sealed class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("Ratings", "engagement");

            // -------------------------
            //           Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<RatingTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //       Foreign Keys
            // -------------------------

            builder.Property(x => x.OrderID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.RaterUserID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.RaterUserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Enums
            // -------------------------

            builder.Property(x => x.TargetType)
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.Stars)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //          Fields
            // -------------------------

            // -------- Relations --------

            builder.Property(x => x.RatedEntityID)
                .IsRequired();

            // -------- Content --------

            builder.Property(x => x.Comment)
                .HasMaxLength(500)
                .IsUnicode(true)
                .IsRequired(false);

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.OrderID);

            builder.HasIndex(x => x.RaterUserID);

            builder.HasIndex(x => new 
            { 
                x.TargetType, 
                x.RatedEntityID, 
                x.CreatedAt 
            });
        }
    }
}