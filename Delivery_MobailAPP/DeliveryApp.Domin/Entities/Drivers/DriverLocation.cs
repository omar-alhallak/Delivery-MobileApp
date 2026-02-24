using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverLocation
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public Guid DriverID { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public DateTimeOffset RecordedAt { get; set; }

        public DriverLocation()
        {
            ID = Guid.NewGuid();
            RecordedAt = DateTimeOffset.UtcNow;
        }
    }
}