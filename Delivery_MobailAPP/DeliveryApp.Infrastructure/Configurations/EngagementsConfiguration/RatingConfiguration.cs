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
            builder.ToTable("Rating", "engagement");

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

            builder.Property(x => x.OrderID) // One(Order) -----> Many(Rating) || الطلب المرتبط بالتقييم
                .HasConversion(
                    id => id.Value,
                    value => StrongID<OrderTag>.From(value))
                .IsRequired();

            builder.HasOne<Order>()
                .WithMany()
                .HasForeignKey(x => x.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------

            builder.Property(x => x.RaterUserID) // One(User) -----> Many(Rating) || المستخدم الي قييم
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.RaterUserID)
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

            builder.HasIndex(x => x.RaterUserID); // جلب كل التقييمات التي كتبها مستخدم معين

            builder.HasIndex(x => new // جلب تقييمات كيان معين (سائق / مطعم / زبون) مرتبة زمنياً
            {
                x.TargetType,
                x.RatedEntityID,
                x.CreatedAt
            });

            builder.HasIndex(x => new // منع تكرار التقييم من نفس المستخدم لنفس الجهة داخل نفس الطلب
            {         
                x.OrderID,
                x.RaterUserID,
                x.TargetType
            })     .IsUnique();
        }
    }
}