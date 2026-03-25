using System;

namespace DeliveryApp.Domain.Enums.EngagementEnams
{
    public enum RelatedEntityType : byte
    {
        Order = 1,
        Complaint = 2,
        AccountWarning = 3,
        DriverRequest = 4
    }
}