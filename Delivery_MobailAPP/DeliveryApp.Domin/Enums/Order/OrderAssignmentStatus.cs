using System;

namespace DeliveryApp.Domain.Enums.Order
{
    public enum OrderAssignmentStatus : byte
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
        Removed = 4
    }
}