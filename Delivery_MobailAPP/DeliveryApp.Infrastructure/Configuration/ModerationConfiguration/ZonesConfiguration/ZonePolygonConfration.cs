using DeliveryApp.Domain.Entities.Moderation.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration.ZonesConfiguration
{
    public sealed class ZonePolygonConfration : IEntityTypeConfiguration<ZonePolygon>
    {

        public void Configure(EntityTypeBuilder<ZonePolygon> builder)
        {
            builder.ToTable("ZonePolygons");

            builder.HasKey("ZoneID", nameof(ZonePolygon.SortOrder));

            // -------------------------
            //         Location
            // -------------------------

            builder.OwnsOne(x => x.Location, location =>
            {
                location.Property(p => p.Latitude)
                    .HasColumnName("Latitude") 
                    .HasPrecision(18, 10)      
                    .IsRequired();

                location.Property(p => p.Longitude)
                    .HasColumnName("Longitude")
                    .HasPrecision(18, 10)
                    .IsRequired();
            });

            // -------------------------
            //       restrictions
            // -------------------------

            builder.Property(x => x.SortOrder)
                .IsRequired();

            // -------------------------
            //           Dates
            // -------------------------
            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();

            // -------------------------
            //          Indexes
            // -------------------------


            builder.HasIndex("ZoneID", nameof(ZonePolygon.SortOrder))
                .IsUnique()
                .HasDatabaseName("UX_ZonePolygon_Zone_Order");

        }
    }
}
