using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.EngagementsConfiguration
{
    public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

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
            //      restrictions
            // -------------------------

            builder.Property(x => x.Body)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.Title)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired(false);


            builder.Property(x => x.IsRead)
                .IsRequired();

            // -------------------------
            //          Enum
            // -------------------------

            builder.Property(x => x.RelatedEntityType)
                .HasConversion<int>() 
                .IsRequired(false);

            // -------------------------
            //       ForeignKey
            // -------------------------

            builder.Property(x => x.UserID)
               .HasConversion(
                   id => id.Value,
                   value => StrongID<UserTag>.From(value))
               .IsRequired();

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserID)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);



            builder.Property(x => x.RelatedEntityID)
              .HasConversion(
                   id => id,
                   value => value
               )
              .IsRequired(false);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.RelatedEntityID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.ReadAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);
        }
    }
}
