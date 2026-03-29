using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configuration.ModerationConfiguration.ZonesConfiguration
{
    public sealed class ZoneConfiguration :IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zones");

            // -------------------------
            //            Key
            // -------------------------
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasConversion(
                    id => id.Value,
                    value => StrongID<ZoneTag>.From(value))
                .ValueGeneratedNever();

            // -------------------------
            //    Personal Information
            // -------------------------

            builder.Property(x => x.ZoneName)
                .HasMaxLength(150)   
                .IsUnicode(true)      
                .IsRequired();        

            builder.Property(x => x.IsActive)
                .IsRequired();       

            builder.Property(x => x.IsServiceable)
                .IsRequired();       

            // -------------------------
            //           Dates
            // -------------------------

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset") 
                .IsRequired();


            builder.HasMany(x => x.Polygons)
                .WithOne()                     
                .HasForeignKey("ZoneID")     
                .OnDelete(DeleteBehavior.Cascade); 


        }
    }
}
