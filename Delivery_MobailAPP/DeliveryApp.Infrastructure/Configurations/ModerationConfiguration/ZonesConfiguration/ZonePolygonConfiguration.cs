using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration.ZonesConfiguration
{
    public sealed class ZonePolygonConfiguration : IEntityTypeConfiguration<ZonePolygon>
    {
        public void Configure(EntityTypeBuilder<ZonePolygon> builder)
        {
            builder.ToTable("ZonePolygons", "moderation");

            // -------------------------
            //            Key
            // -------------------------

            builder.HasKey("ZoneID", nameof(ZonePolygon.SortOrder));

            // -------------------------
            //           Fields
            // -------------------------

            // -------- Location --------

            builder.OwnsOne(x => x.Location, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("Latitude")
                    .HasPrecision(9, 6)
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("Longitude")
                    .HasPrecision(9, 6)
                    .IsRequired();
            });

            // -------- Order --------

            builder.Property(x => x.SortOrder)
                .IsRequired();

            // -------- Dates --------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();
        }
    }
}