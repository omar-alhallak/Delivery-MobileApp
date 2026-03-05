using System;

namespace DeliveryApp.Domain.DomainErrors.Drivers
{
    public static class DriverRequestErrors
    {
        public const string NotPendingCode = "DriverRequest.NotPending";
        public const string NotPendingMessage = "Only pending driver requests can be modified.";

        public const string VehicleDetailsRequiredCode = "DriverRequest.VehicleDetailsRequired";
        public const string VehicleDetailsRequiredMessage = "Vehicle details are required before approval.";
    }
}