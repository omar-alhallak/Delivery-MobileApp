using System;

namespace DeliveryApp.Domain.Enums.Order
{
    public enum CancelledByType : byte
    {
        Customer = 1,
        Driver = 2,
        Merchant = 3,
        Admin = 4,
    }
}