using System;

namespace DeliveryApp.Domain.Enums.Order
{
    public enum OrderType : byte
    {
        Near = 1,
        Far = 2,
        Shipping = 3
    }
}