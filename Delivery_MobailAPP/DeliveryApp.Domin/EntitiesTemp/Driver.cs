using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class Driver
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserID { get; set; }

        [Required]
        public Guid VehicleTypeID { get; set; }

        public bool IsApproved { get; set; }

        public bool IsOnline { get; set; }

        public bool IsAvailable { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? CurrentLat { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? CurrentLng { get; set; }

        public int ActiveOrdersCount { get; set; }

        public Guid? ApprovedByAdminId { get; set; }

        public DateTimeOffset? ApprovedAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Driver()
        {
            CreatedAt = DateTimeOffset.UtcNow;
            IsApproved = false;
            IsOnline = false;
            IsAvailable = false;
            ActiveOrdersCount = 0;
        }
    }
}