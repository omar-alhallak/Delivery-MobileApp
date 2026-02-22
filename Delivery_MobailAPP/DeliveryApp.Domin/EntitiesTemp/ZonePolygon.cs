using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class ZonePolygon
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public Guid ZoneID { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        private ZonePolygon() { } 

        public ZonePolygon(Guid ZoneId, decimal latitude, decimal longitude, int sortOrder)
        {
            ID = Guid.NewGuid();
            ZoneID = ZoneId;
            Latitude = latitude;
            Longitude = longitude;
            SortOrder = sortOrder;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}