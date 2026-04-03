using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Engagements;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.EngagementsConfiguration
{
    public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", "engagement");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<NotificationTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.UserID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //            Enums
            // -------------------------

            builder.Property(x => x.RelatedEntityType)
                .HasConversion<byte?>()
                .IsRequired(false);

            // -------------------------
            //            Fields
            // -------------------------

            // -------- Content --------

            builder.Property(x => x.Title)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            builder.Property(x => x.Body)
                .HasMaxLength(1000)
                .IsUnicode(true)
                .IsRequired();

            // -------- Related Entity --------

            builder.Property(x => x.RelatedEntityID)
                .IsRequired(false);

            // -------- Status --------

            builder.Property(x => x.IsRead)
                .IsRequired();

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.ReadAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //            Indexes
            // -------------------------

            builder.HasIndex(x => x.UserID);

            builder.HasIndex(x => new { x.RelatedEntityType, x.RelatedEntityID });
        }
    }
}