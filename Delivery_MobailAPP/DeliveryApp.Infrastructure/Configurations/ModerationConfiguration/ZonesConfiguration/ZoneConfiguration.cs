using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration.ZonesConfiguration
{
    public sealed class ZoneConfiguration : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zones", "moderation");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<ZoneTag>.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            // -------------------------
            //       Relationships
            // -------------------------

            builder.HasMany(x => x.Polygons)
                .WithOne()
                .HasForeignKey("ZoneID")
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Zone Info --------

            builder.Property(x => x.ZoneName)
                .HasMaxLength(150)
                .IsUnicode(true)
                .IsRequired();

            // -------- Status --------

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.IsServiceable)
                .IsRequired();

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //           Items
            // -------------------------

            builder.Metadata.FindNavigation(nameof(Zone.Polygons))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => x.IsServiceable)
                .HasFilter("[IsActive] = 1 AND [IsServiceable] = 1");
        }
    }
}