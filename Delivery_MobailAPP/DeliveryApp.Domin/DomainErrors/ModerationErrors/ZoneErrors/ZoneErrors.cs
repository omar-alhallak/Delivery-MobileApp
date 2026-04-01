namespace DeliveryApp.Domain.DomainErrors.ModerationErrors.ZoneErrors
{
    public static class ZoneErrors
    {
        // لا يمكن ترتيب نقطتين سوا
        public const string DuplicatePolygonOrderCode = "Zone.Duplicate_Polygon_Order";
        public const string DuplicatePolygonOrderMessage = "Polygon point order already exists.";

        // لا يمكن إضافة النقطة مرتين
        public const string DuplicatePolygonLocationCode = "Zone.Duplicate_Polygon_Location";
        public const string DuplicatePolygonLocationMessage = "Polygon point location already exists.";

        // لا يمكن حذف نقطة غير موجودة
        public const string PointNotFoundCode = "Zone.Polygon_Point_Not_Found";
        public const string PointNotFoundMessage = "Polygon point was not found.";

        // يجب أن يكون 3 نثاط على الأقل
        public const string PolygonRequiresMinimumPointsCode = "Zone.Polygon_Minimum_Points";
        public const string PolygonRequiresMinimumPointsMessage = "Zone polygon must contain at least 3 points.";

        // لا يمكن أن تكون النقطة غير مفعلة ومخدمة سوا
        public const string InactiveZoneCantBeServiceableCode = "Zone.Inactive_Cant_Be_Serviceable";
        public const string InactiveZoneCantBeServiceableMessage = "Inactive zone cant be marked as serviceable.";
    }
}