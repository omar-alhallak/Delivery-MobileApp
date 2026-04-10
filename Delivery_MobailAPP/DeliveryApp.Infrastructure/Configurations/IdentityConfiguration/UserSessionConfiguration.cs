using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Infrastructure.Configurations.IdentityConfiguration
{
    public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions", "identity");

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

            builder.Property(x => x.UserID)
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

            builder.Property(x => x.RefreshTokenHash)
                .HasConversion(
                    value => value.ToArray(),
                    value => new ReadOnlyMemory<byte>(value))
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

            builder.HasIndex(x => x.UserID);

            builder.HasIndex(x => new
            {
                x.UserID,
                x.ClientType
            }).IsUnique();

            builder.HasIndex(x => x.RefreshTokenHash)
                .IsUnique();

            builder.HasIndex(x => x.ExpiresAt);
        }
    }
}