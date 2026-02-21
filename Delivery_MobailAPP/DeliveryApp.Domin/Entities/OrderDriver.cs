using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    public enum OrderDriverStatus
    {
        Assigned = 1,
        Accepted = 2,
        Rejected = 3,
        Completed = 4,
        Removed = 5
    }
    internal class OrderDriver
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }

        public Guid DriverId { get; private set; }
        
        public DateTimeOffset AssignedAt { get; private set; }

        public OrderDriverStatus Status { get; private set; }

        public DateTimeOffset RemovedAt { get; private set; }

        private OrderDriver() { }


        public OrderDriver(Guid orderId, Guid driverId)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            DriverId = driverId;
            AssignedAt = DateTimeOffset.UtcNow;
            Status = OrderDriverStatus.Assigned;
        }


        public void UpdateStatus(OrderDriverStatus newStatus, string? reason = null)
        {

            Status = newStatus;

            if (newStatus == OrderDriverStatus.Removed)
            {
                RemovedAt = DateTimeOffset.UtcNow;
            }
        }
        

    }
}
