using DeliveryApp.Domain.DomainErrors.ZoneErrors;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.Moderation.Zones;
using DeliveryApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Moderation.Zones
{
    public class Zone
    {
        public ZoneID ID { get; private set; }
        public string ZoneName { get; private set; } = null!;

        public bool IsActive { get; private set; }
        public bool IsServiceable { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private readonly List<ZonePolygon> polygons = new();
        public IReadOnlyCollection<ZonePolygon> Polygons => polygons.AsReadOnly();

        private Zone() { }

        public Zone(ZoneID id, string zoneName, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ID = id;
            SetName(zoneName);

            IsActive = true;
            IsServiceable = false;
            CreatedAt = createdAtUtc;
        }

        //         Behavior
        public void Rename(string zoneName)
        {
            SetName(zoneName);
        }

        public void Activate()
        {
            /// If the zone is already active, we can skip the operation to avoid unnecessary state changes.
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            IsServiceable = false;
        }

        public void MarkAsServiceable()
        {
            if (IsServiceable) return;

            if (!IsActive) throw new DomainRuleViolationException
                    (ZoneErrors.InactiveZoneCannotBeServiceableCode,ZoneErrors.InactiveZoneCannotBeServiceableMessage);

            EnsureValidPolygon();

            IsServiceable = true;
        }

        public void MarkAsNotServiceable()
        {
            if (!IsServiceable) return;

            IsServiceable = false;
        }

        //إضافة نقطة إلى المضلع
        public void AddPolygonPoint(GeoPoint location, int sortOrder, DateTimeOffset createdAtUtc)
        {
            if (location is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(location));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            //التحقق من صحة البيانات المدخلة
            if (sortOrder <= 0) throw new DomainValidationException
            (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(sortOrder));

            //التحقق من عدم وجود نقطة بنفس ترتيب العرض
            if (polygons.Any(x => x.SortOrder == sortOrder)) throw new DomainRuleViolationException
                    (ZoneErrors.DuplicatePolygonOrderCode, ZoneErrors.DuplicatePolygonOrderMessage);

            //التحقق من عدم وجود نقطة بنفس الموقع
            if (polygons.Any(x => x.Location.Equals(location))) throw new DomainRuleViolationException
                    ( ZoneErrors.DuplicatePolygonLocationCode, ZoneErrors.DuplicatePolygonLocationMessage);

            polygons.Add(new ZonePolygon(location, sortOrder, createdAtUtc));
        }
        //إزالة نقطة من المضلع
        public void RemovePolygonPoint(int sortOrder)
        {
            var polygon = polygons.FirstOrDefault(x => x.SortOrder == sortOrder);
            if (polygon is null) throw new DomainRuleViolationException
                   (ZoneErrors.PointNotFoundCode, ZoneErrors.PointNotFoundMessage);

            polygons.Remove(polygon);
        }
        //تغيير ترتيب نقطة في المضلع
        public void ChangePolygonPointOrder(int currentSortOrder, int newSortOrder)
        {
            
            if (newSortOrder <= 0) throw new DomainValidationException
                   (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(newSortOrder));

            var polygon = polygons.FirstOrDefault(x => x.SortOrder == currentSortOrder);

            if (polygon is null) throw new DomainRuleViolationException
                    (ZoneErrors.PointNotFoundCode, ZoneErrors.PointNotFoundMessage);

            if (polygons.Any(x => x.SortOrder == newSortOrder)) throw new DomainRuleViolationException
                   (ZoneErrors.DuplicatePolygonOrderCode, ZoneErrors.DuplicatePolygonOrderMessage);

            polygon.ChangeSortOrder(newSortOrder);
        }
        //تغيير موقع نقطة في المضلع
        public void ChangePolygonPointLocation(int sortOrder, GeoPoint location)
        {
            if (location is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(location));

            var polygon = polygons.FirstOrDefault(x => x.SortOrder == sortOrder);
            if (polygon is null) throw new DomainRuleViolationException
                    (ZoneErrors.PointNotFoundCode, ZoneErrors.PointNotFoundMessage);

            polygon.ChangeLocation(location);
        }

        //Setters
        private void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ZoneName));

            value = value.Trim();

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ZoneName));

            ZoneName = value;
        }
        //Helpers
        private void EnsureValidPolygon()
        {
                if (polygons.Count < 3) throw new DomainRuleViolationException
                 (ZoneErrors.PolygonRequiresMinimumPointsCode, ZoneErrors.PolygonRequiresMinimumPointsMessage);
        }
    }
}