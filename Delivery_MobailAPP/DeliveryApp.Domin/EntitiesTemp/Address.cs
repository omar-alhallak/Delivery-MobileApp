using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class Address
    {
        [Key]
        public Guid AddressID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        [Required]
        public int AddressType { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [MaxLength(200)]
        public string? BuildingName { get; set; }

        [MaxLength(50)]
        public string? Floor { get; set; }

        [MaxLength(100)]
        public string? DoorInfo { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Address()
        {
            AddressID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsDefault = false;
        }
    }
}