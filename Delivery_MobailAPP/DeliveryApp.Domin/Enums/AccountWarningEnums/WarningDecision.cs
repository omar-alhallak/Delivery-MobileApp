using System;

namespace DeliveryApp.Domain.Enums.AccountWarningEnums
{
    public enum WarningDecision : byte
    {
        Pending = 1,
        Confirmed = 2,
        Dismissed = 3
    }
}