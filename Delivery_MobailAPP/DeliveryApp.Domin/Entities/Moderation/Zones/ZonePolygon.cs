using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Moderation.Zones
{
    public class ZonePolygon
    {
        public GeoPoint Location { get; private set; } = null!;
        public int SortOrder { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private ZonePolygon() { }

        public ZonePolygon(GeoPoint location, int sortOrder, DateTimeOffset createdAtUtc)
        {
            if (location is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(location));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            Location = location;
            CreatedAt = createdAtUtc;

            SetSortOrder(sortOrder);
        }

        public void ChangeSortOrder(int sortOrder) => SetSortOrder(sortOrder);

        public void ChangeLocation(GeoPoint location)
        {
            if (location is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(location));

            if (Location.Equals(location)) return;

            Location = location;
        }

        private void SetSortOrder(int value)
        {
            if (value <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(SortOrder));

            SortOrder = value;
        }
    }
}