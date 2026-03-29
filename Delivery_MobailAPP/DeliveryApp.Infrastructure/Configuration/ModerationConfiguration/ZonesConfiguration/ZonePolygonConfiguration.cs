using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.ModerationConfiguration.ZonesConfiguration
{
    public sealed class ZonePolygonConfiguration : IEntityTypeConfiguration<ZonePolygon>
    {

        public void Configure(EntityTypeBuilder<ZonePolygon> builder)
        {
            builder.ToTable("ZonePolygons");

            builder.HasKey("Id"); 


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


            builder.Property(x => x.SortOrder)
                .IsRequired();

            // -------------------------
            //           Dates
            // -------------------------
            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .IsRequired();
        }
    }
}
