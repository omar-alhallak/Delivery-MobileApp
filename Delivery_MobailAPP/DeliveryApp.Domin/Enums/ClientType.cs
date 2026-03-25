using System;

namespace DeliveryApp.Domain.Enums
{
    public enum ClientType : byte
    {
        CustomerApp = 1,
        DriverApp = 2,
        MerchantWebApp = 3,
        AdminDashboard = 4
    }
}