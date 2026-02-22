using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class Zone
    {
        [Key]
        public Guid ZoneID { get; set; }

        [Required]
        [MaxLength(150)]
        public string ZoneName { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool IsServiceable { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Zone() { }

        public Zone(string zoneName, bool isServiceable)
        {
            ZoneID = Guid.NewGuid();
            ZoneName = zoneName.Trim();
            IsActive = true;
            IsServiceable = isServiceable;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}