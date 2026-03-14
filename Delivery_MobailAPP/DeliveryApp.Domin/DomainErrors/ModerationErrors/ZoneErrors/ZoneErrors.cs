using System;

namespace DeliveryApp.Domain.DomainErrors.ModerationErrors.ZoneErrors
{
    public class ZoneErrors
    {
        public const string DuplicatePolygonOrderCode = "Zone.Duplicate_Polygon_Order";
        public const string DuplicatePolygonOrderMessage = "Polygon point order already exists.";

        public const string DuplicatePolygonLocationCode = "Zone.Duplicate_Polygon_Location";
        public const string DuplicatePolygonLocationMessage = "Polygon point location already exists.";

        public const string PointNotFoundCode = "Zone.Polygon_Point_Not_Found";
        public const string PointNotFoundMessage = "Polygon point was not found.";

        public const string PolygonRequiresMinimumPointsCode = "Zone.Polygon_Minimum_Points";
        public const string PolygonRequiresMinimumPointsMessage = "Zone polygon must contain at least 3 points.";

        public const string InactiveZoneCannotBeServiceableCode = "Zone.Inactive_Cannot_Be_Serviceable";
        public const string InactiveZoneCannotBeServiceableMessage = "Inactive zone cannot be marked as serviceable.";
    }
}