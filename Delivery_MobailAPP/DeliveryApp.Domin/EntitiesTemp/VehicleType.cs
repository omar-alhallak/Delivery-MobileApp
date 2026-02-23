using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class VehicleType
    {
        [Key]
        public int VehicleID { get; set; }

        [Required]
        [MaxLength(100)]
        public string VehicleName { get; set; } = string.Empty;

        [Required]
        [Range(0, 1000)]
        public double MaxDistanceKm { get; set; }

        [Required]
        [Range(0, 1000)]
        public double MaxMergeExtraKm { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal CommissionPercent { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public VehicleType()
        {
            IsActive = true;
        }
    }
}