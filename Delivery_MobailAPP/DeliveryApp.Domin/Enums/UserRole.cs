using System;

namespace DeliveryApp.Domain.Enums
{
    [Flags]
    public enum UserRole : int
    {
        None = 0,
        Customer = 1 << 0, // 1
        Driver = 1 << 1, // 2
        Admin = 1 << 2, // 4
    }
}