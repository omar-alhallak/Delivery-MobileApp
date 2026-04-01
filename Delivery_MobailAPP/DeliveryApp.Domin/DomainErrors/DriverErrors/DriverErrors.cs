namespace DeliveryApp.Domain.DomainErrors.DriverErrors
{
    public static class DriverErrors
    {
        // السائق موقف
        public const string DisabledCode = "Driver.Disabled";
        public const string DisabledMessage = "Driver app access is disabled.";

        // السائق غير متصل
        public const string OfflineCode = "Driver.Offline";
        public const string OfflineMessage = "Driver must be online.";

        // إذا السائق كان يوصل طلب لا يمكن أن يجعل نفسه غير متوفر
        public const string CantbeUnavailableWithActiveOrdersCode = "Driver.Cant_Go_Unavailable_With_Active_Orders";
        public const string CantbeUnavailableWithActiveOrdersMessage = "Cant go unavailable while having active orders.";
    }
}