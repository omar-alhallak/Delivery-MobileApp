using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.ModerationErrors.ZoneErrors;

namespace DeliveryApp.Domain.Entities.Moderation.Zones
{
    public class Zone // يمثل منطقة توصيل وحدودها الجغرافية
    {
        // -------------------------
        //            Key
        // -------------------------

        public ZoneID ID { get; private set; } // PK معرف المنطقة

        // -------------------------
        //       Zone Info
        // -------------------------

        public string ZoneName { get; private set; } = null!; // اسم المنطقة

        // -------------------------
        //         Status
        // -------------------------

        public bool IsActive { get; private set; } // هل المنطقة مفعلة
        public bool IsServiceable { get; private set; } // هل المنطقة صالحة للخدمة والتوصيل

        // -------------------------
        //          Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء المنطقة

        // -------------------------
        //         Polygon
        // -------------------------

        private readonly List<ZonePolygon> polygons = new(); // نقاط حدود المنطقة
        public IReadOnlyCollection<ZonePolygon> Polygons => polygons.AsReadOnly(); // عرض النقاط

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

        // -------------------------
        //         Behavior
        // -------------------------

        public void Rename(string zoneName) => SetName(zoneName); // تغيير اسم المنطقة

        public void Activate() // تفعيل المنطقة
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate() // تعطيل المنطقة
        {
            if (!IsActive) return;

            IsActive = false;
            IsServiceable = false;
        }

        public void MarkAsServiceable() // جعل المنطقة مخدمة
        {
            if (IsServiceable) return;

            if (!IsActive) throw new DomainRuleViolationException
                    (ZoneErrors.InactiveZoneCantBeServiceableCode, ZoneErrors.InactiveZoneCantBeServiceableMessage);

            EnsureMinimumPolygonPoints();
            IsServiceable = true;
        }

        public void MarkAsNotServiceable() // إلغاء تخديم المنطقة
        {
            if (!IsServiceable) return;

            IsServiceable = false;
        }

        public void AddPolygonPoint(GeoPoint location, int sortOrder, DateTimeOffset createdAtUtc) // إضافة نقطة جديدة إلى حدود المنطقة
        {
            if (location is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(location));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (sortOrder <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(sortOrder));

            if (polygons.Any(x => x.SortOrder == sortOrder)) throw new DomainRuleViolationException
                    (ZoneErrors.DuplicatePolygonOrderCode, ZoneErrors.DuplicatePolygonOrderMessage);

            if (polygons.Any(x => x.Location.Equals(location))) throw new DomainRuleViolationException
                    (ZoneErrors.DuplicatePolygonLocationCode, ZoneErrors.DuplicatePolygonLocationMessage);

            polygons.Add(new ZonePolygon(location, sortOrder, createdAtUtc));
        }

        public void RemovePolygonPoint(int sortOrder) // إزالة نقطة من حدود المنطقة
        {
            var polygon = polygons.FirstOrDefault(x => x.SortOrder == sortOrder);

            if (polygon is null) throw new DomainRuleViolationException
                    (ZoneErrors.PointNotFoundCode, ZoneErrors.PointNotFoundMessage);

            polygons.Remove(polygon);

            if (polygons.Count < 3)
                IsServiceable = false;
        }

        public void ChangePolygonPointOrder(int currentSortOrder, int newSortOrder) // تغيير ترتيب نقطة ضمن حدود المنطقة
        {
            if (currentSortOrder == newSortOrder) return;

            if (newSortOrder <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(newSortOrder));

            var polygon = polygons.FirstOrDefault(x => x.SortOrder == currentSortOrder);

            if (polygon is null) throw new DomainRuleViolationException
                    (ZoneErrors.PointNotFoundCode, ZoneErrors.PointNotFoundMessage);

            if (polygons.Any(x => x.SortOrder == newSortOrder)) throw new DomainRuleViolationException
                    (ZoneErrors.DuplicatePolygonOrderCode, ZoneErrors.DuplicatePolygonOrderMessage);

            polygon.ChangeSortOrder(newSortOrder);
        }

        public void ChangePolygonPointLocation(int sortOrder, GeoPoint location) // تغيير موقع نقطة موجودة ضمن المنطقة
        {
            if (location is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(location));

            var polygon = polygons.FirstOrDefault(x => x.SortOrder == sortOrder);

            if (polygon is null) throw new DomainRuleViolationException
                    (ZoneErrors.PointNotFoundCode, ZoneErrors.PointNotFoundMessage);

            if (polygon.Location.Equals(location)) return;

            if (polygons.Any(x => x.SortOrder != sortOrder && x.Location.Equals(location)))
                throw new DomainRuleViolationException
                    (ZoneErrors.DuplicatePolygonLocationCode, ZoneErrors.DuplicatePolygonLocationMessage);

            polygon.ChangeLocation(location);
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetName(string value) // إدخال اسم المنطقة
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ZoneName));

            value = value.Trim();

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ZoneName));

            ZoneName = value;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        private void EnsureMinimumPolygonPoints() // التأكد أن الحدود تحتوي على الحد الأدنى من النقاط
        {
            if (polygons.Count < 3) throw new DomainRuleViolationException
                    (ZoneErrors.PolygonRequiresMinimumPointsCode, ZoneErrors.PolygonRequiresMinimumPointsMessage);
        }
    }
}