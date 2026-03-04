using System;

namespace DeliveryApp.Domain.Enums
{
    public enum AccountStatus : byte
    {
        Active = 1,
        Suspended = 2,
        Banned = 3,
    }
}