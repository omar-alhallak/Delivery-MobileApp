using System;

namespace DeliveryApp.Domain.Enums.ComplaintEnums
{
    public enum ComplaintStatus : byte
    {
        Pending = 1,
        Resolved = 2,
        Rejected = 3
    }
}