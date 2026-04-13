using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.IdentityConfiguration
{
    public sealed class UserIdentityConfiguration : IEntityTypeConfiguration<UserIdentity>
    {
        public void Configure(EntityTypeBuilder<UserIdentity> builder)
        {
            builder.ToTable("UserIdentity", "identity");

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
            //        Foreign Keys
            // -------------------------

            builder.Property(x => x.UserID) // One(User) -----> Many(UserIdentity) || المستخدم المرتبط بالهوية
                .HasConversion(
                    id => id.Value,
                    value => StrongID<UserTag>.From(value))
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            //           Enums
            // -------------------------

            builder.Property(x => x.Provider)
                .HasConversion<byte>()
                .IsRequired();

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Provider Info --------

            builder.Property(x => x.ProviderUserId)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(x => x.PasswordHash)
                .HasMaxLength(300)
                .IsUnicode(false)
                .IsRequired(false);

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new // منع تكرار نفس الحساب مع أكثر من مستخدم
            {
                x.Provider,
                x.ProviderUserId
            })  .IsUnique()
                .HasFilter("[ProviderUserId] IS NOT NULL");

            builder.HasIndex(x => new // منع المستخدم من أنو يكون عنده نفس طريقة تسجيل مكررة
            {
                x.UserID,
                x.Provider
            })  .IsUnique();
        }
    }
}