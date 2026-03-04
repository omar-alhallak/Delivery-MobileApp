using System;

namespace DeliveryApp.Domain.DomainErrors.DriverErrors
{
    public static class DriverErrors
    {
        public const string DisabledCode = "driver.disabled";
        public const string DisabledMessage = "Driver app access is disabled.";

        public const string OfflineCode = "driver.offline";
        public const string OfflineMessage = "Driver must be online.";

        public const string CantbeUnavailableWithActiveOrdersCode = "Driver.cannot_Go_Unavailable_With_Active_Orders";
        public const string CantbeUnavailableWithActiveOrdersMessage = "Cannot go unavailable while having active orders.";
    }
}