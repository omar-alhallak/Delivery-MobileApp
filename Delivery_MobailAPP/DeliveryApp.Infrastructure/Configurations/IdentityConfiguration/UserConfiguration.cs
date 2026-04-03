using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.IdentityConfiguration
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "identity");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //         Public ID
            // -------------------------

            builder.Property(x => x.PublicID)
                .HasConversion(
                    value => value.HasValue ? value.Value.Value : null,
                    value => string.IsNullOrWhiteSpace(value) ? null : PublicCode.From(value))
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsRequired(false);

            // -------------------------
            //           Enums
            // -------------------------

            builder.Property(x => x.RoleMask)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.AccountStatus)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Personal Information --------

            builder.Property(x => x.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.FullName)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired(false);

            builder.Property(x => x.Phone)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.PhotoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.IsProfileComplete)
                .IsRequired();

            // -------- Status --------

            builder.Property(x => x.SuspendedUntilUtc)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------- Rating --------

            builder.Property(x => x.CustomerAverageRating)
                .HasPrecision(3, 2)
                .IsRequired();

            builder.Property(x => x.CustomerRatingsCount)
                .IsRequired();

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(x => x.LastLoginAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.PublicID)
                .IsUnique()
                .HasFilter("[PublicID] IS NOT NULL");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");
                    
            builder.HasIndex(x => x.Phone)
                .IsUnique()
                .HasFilter("[Phone] IS NOT NULL");
        }
    }
}