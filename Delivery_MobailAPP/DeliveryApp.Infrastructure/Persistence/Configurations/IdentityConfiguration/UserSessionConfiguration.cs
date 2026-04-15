using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Persistence.Configurations.IdentityConfiguration
{
    public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSession", "identity");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserSessionTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.UserID) // One(User) -----> Many(UserSession) || المستخدم صاحب الجلسة
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //            Enums
            // -------------------------

            builder.Property(x => x.ClientType)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Client Info --------

            builder.Property(x => x.DeviceID)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsRequired();

            // -------- Refresh Token Hash --------

            builder.Property<byte[]>("refreshTokenHash")
                .HasColumnName("RefreshTokenHash")
                .HasColumnType("varbinary(32)")
                .IsRequired();
                       
            // -------- Dates --------
                 
            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.LastSeenAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.RevokedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new // منع وجود أكثر من جلسة لنفس المستخدم على نفس التطبيق
            {
                x.UserID,
                x.ClientType
            })  .IsUnique();

            builder.HasIndex("refreshTokenHash") // البحث عن الجلسة من خلال
                .IsUnique();                          // Refresh Token Hash ومنع تكراره

            builder.HasIndex(x => x.ExpiresAt); // جلب الجلسات المنتهية
        }
    }
}