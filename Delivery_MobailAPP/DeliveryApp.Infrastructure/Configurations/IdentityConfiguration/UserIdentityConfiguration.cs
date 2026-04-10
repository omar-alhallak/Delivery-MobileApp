using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Infrastructure.Configurations.IdentityConfiguration
{
    public sealed class UserIdentityConfiguration : IEntityTypeConfiguration<UserIdentity>
    {
        public void Configure(EntityTypeBuilder<UserIdentity> builder)
        {
            builder.ToTable("UserIdentities", "identity");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserIdentityTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //         Relations
            // -------------------------

            builder.Property(x => x.UserID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            //           Enums
            // -------------------------

            builder.Property(x => x.Provider)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            builder.Property(x => x.ProviderUserId)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.PasswordHash)
                .HasMaxLength(300)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            // منع تكرار نفس الحساب من نفس المزود
            builder.HasIndex(x => new { x.Provider, x.ProviderUserId })
                .IsUnique()
                .HasFilter("[ProviderUserId] IS NOT NULL");

            // ممكن مستخدم عنده مزود واحد مرة وحدة
            builder.HasIndex(x => new { x.UserID, x.Provider })
                .IsUnique();
        }
    }
}