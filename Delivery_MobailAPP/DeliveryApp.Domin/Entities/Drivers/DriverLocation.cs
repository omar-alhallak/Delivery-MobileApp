using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverLocation // يمثل سجل موقع السائق في وقت معين
    {
        // -------------------------
        //            Key
        // -------------------------

        public DriverLocationID ID { get; private set; } // PK معرف سجل الموقع

        // -------------------------
        //         Relations
        // -------------------------

        public UserID DriverID { get; private set; } // السائق الي يخصه هذا السجل

        // -------------------------
        //          Location
        // -------------------------

        public GeoPoint Location { get; private set; } = null!; // موقع السائق

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset RecordedAt { get; private set; } // وقت تسجيل هذا الموقع

        private DriverLocation() { }

        public DriverLocation(UserID driverId, double latitude, double longitude, DateTimeOffset recordedAtUtc) 
        {
            if (driverId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(driverId));

            if (recordedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(recordedAtUtc));

            ID = DriverLocationID.New();
            DriverID = driverId;
            Location = GeoPoint.Create(latitude, longitude);
            RecordedAt = recordedAtUtc;
        }
    }
}