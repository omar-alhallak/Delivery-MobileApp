using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverLocation
    {
        public Guid ID { get; private set; }
        public UserID DriverID { get; private set; }

        public GeoPoint Location { get; private set; } = null!;
        public DateTimeOffset RecordedAt { get; private set; }

        private DriverLocation() { }

        public DriverLocation(UserID DriverId, decimal latitude, decimal longitude, DateTimeOffset RecordedAtUtc)
        {
            if (DriverId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(DriverId));

            if (RecordedAtUtc == default) throw new DomainValidationException
                     (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(RecordedAtUtc));

            ID = Guid.NewGuid();
            DriverID = DriverId;
            Location = GeoPoint.Create(latitude, longitude);
            RecordedAt = RecordedAtUtc;
        }
    }
}