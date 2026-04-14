namespace DeliveryApp.Domain.DomainErrors.DriverErrors
{
    public static class DriverRequestErrors
    {
        // لا يمكن تعديل الطلب تقديم لانه تم معالجته
        public const string NotPendingCode = "DriverRequest.NotPending";
        public const string NotPendingMessage = "Only pending driver requests can be modified.";

        // معلومات المركبة مطلوبة قبل الموافقة
        public const string VehicleDetailsRequiredCode = "DriverRequest.Vehicle_Details_Required";
        public const string VehicleDetailsRequiredMessage = "Vehicle details are required before approval.";
    }
}