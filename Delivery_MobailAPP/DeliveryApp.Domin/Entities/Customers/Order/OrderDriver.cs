using DeliveryApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Customers.Order
{

    public class OrderDriver
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public Guid OrderID { get; set; }

        [Required]
        public Guid DriverID { get; set; }

        [Required]
        public DateTimeOffset AssignedAt { get; set; }

        [Required]
        [MaxLength(50)]
        public OrderDriverStatus Status { get; set; } 

        public DateTimeOffset? RemovedAt { get; set; }

        [MaxLength(300)]
        public string? RemoveReason { get; set; }

        public OrderDriver()
        {
            ID = Guid.NewGuid();
            AssignedAt = DateTimeOffset.UtcNow;
        }
    }
}