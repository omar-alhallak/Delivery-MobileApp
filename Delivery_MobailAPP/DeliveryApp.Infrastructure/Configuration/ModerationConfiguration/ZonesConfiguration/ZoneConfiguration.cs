using DeliveryApp.Domain.Entities.Moderation.Zones;
using DeliveryApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Configurations.ModerationConfiguration.ZonesConfiguration
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
                .ValueGeneratedNever()
                .IsRequired();

            // -------------------------
            //       restrictions
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

            // -------------------------
            //          Indexes
            // -------------------------

            builder.HasIndex(x => new { x.IsActive, x.IsServiceable })
               .HasFilter("[IsActive] = 1 AND [IsServiceable] = 1")
               .HasDatabaseName("IX_Zone_OperatingOnly");
        }
    }
}
