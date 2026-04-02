using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Orders;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.EngagementsConfiguration
{
    public sealed class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("Ratings");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<RatingTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //      restrictions
            // -------------------------

            builder.Property(x => x.Comment)
               .HasMaxLength(500)
               .IsUnicode(false)
               .IsRequired(false);

            // -------------------------
            //          Enum
            // -------------------------

            builder.Property(x => x.TargetType)
                .HasConversion<int>()
                .IsRequired(false); 

            builder.Property(x => x.Stars)
                .HasConversion<int>()
                .IsRequired(false);

            // -------------------------
            //       ForeignKey
            // -------------------------

            builder.Property(x => x.OrderID)
             .HasConversion(
                 id => id.Value,
                 value => StrongID<OrderTag>.From(value))
             .IsRequired();

            builder.HasOne<Order>()
               .WithMany()
               .HasForeignKey(x => x.OrderID)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.RaterUserID)
            .HasConversion(
                id => id.Value,
                value => StrongID<UserTag>.From(value))
            .IsRequired();

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.RaterUserID)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.RatedEntityID)
               .HasConversion(
                   id => id,
                   value => value
                )
               .IsRequired(false);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.RatedEntityID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

        }
    }
}
