using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;


namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverLocation
    {
        public Guid ID { get; private set; }
        public UserID DriverID { get; private set; }

        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public DateTimeOffset RecordedAt { get; private set; }

        private DriverLocation() { }

        public DriverLocation(UserID DriverId, decimal latitude, decimal longitude, DateTimeOffset RecordedAtUtc)
        {
            if (DriverId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(DriverId));

            ValidateLocation(latitude, longitude);

            if (RecordedAtUtc == default) throw new DomainValidationException
                     (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(RecordedAtUtc));

            ID = Guid.NewGuid();
            DriverID = DriverId;
            Latitude = latitude;
            Longitude = longitude;
            RecordedAt = RecordedAtUtc;
        }

        private static void ValidateLocation(decimal lat, decimal lng)
        {
            if (lat < -90 || lat > 90) throw new DomainValidationException
                    (ValidationErrors.InvalidLatCode, ValidationErrors.InvalidLatMessage, field: nameof(lat));

            if (lng < -180 || lng > 180) throw new DomainValidationException
                    (ValidationErrors.InvalidLngCode, ValidationErrors.InvalidLngMessage, field: nameof(lng));
        }
    }
}