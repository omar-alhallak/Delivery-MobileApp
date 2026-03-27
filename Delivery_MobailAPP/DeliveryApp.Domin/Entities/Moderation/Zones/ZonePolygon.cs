using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Moderation.Zones
{
    public class ZonePolygon // يمثل نقطة ضمن حدود منطقة
    {
        // -------------------------
        //         Location
        // -------------------------

        public GeoPoint Location { get; private set; } = null!; // إحداثيات النقطة

        // -------------------------
        //          Order
        // -------------------------

        public int SortOrder { get; private set; } // ترتيب النقطة ضمن الشكل

        // -------------------------
        //          Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء النقطة

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

        // -------------------------
        //         Behavior
        // -------------------------

        public void ChangeSortOrder(int sortOrder) // تغيير ترتيب النقطة داخل الشكل
        {
            if (SortOrder == sortOrder) return;

            SetSortOrder(sortOrder);
        }

        public void ChangeLocation(GeoPoint location) // تغيير موقع النقطة
        {
            if (location is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(location));

            if (Location.Equals(location)) return;

            Location = location;
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetSortOrder(int value) // إدخال ترتيب النقطة
        {
            if (value <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(value));

            SortOrder = value;
        }
    }
}