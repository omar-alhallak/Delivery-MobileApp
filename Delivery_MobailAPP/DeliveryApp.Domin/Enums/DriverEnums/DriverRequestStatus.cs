using System;

namespace DeliveryApp.Domain.Enums.DriverEnums
{
    public enum DriverRequestStatus : byte
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}