using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.DomainErrors.ZoneErrors
{
    public class ZoneErrors
    {
        // ترتيب نقاط المضلع مكرر
        //public const string InvalidPoint = "Zone.Duplicate_Sort_Order";
        //public const string InvalidPointMessage = "Polygon point sort order must be unique within the zone.";
        // ترتيب نقاط المضلع مكرر
        public const string DuplicatePolygonOrderCode = "Zone.Duplicate_Polygon_Order";
        public const string DuplicatePolygonOrderMessage = "Polygon point order already exists.";
        // موقع نقاط المضلع مكرر
        public const string DuplicatePolygonLocationCode = "Zone.Duplicate_Polygon_Location";
        public const string DuplicatePolygonLocationMessage = "Polygon point location already exists.";
        // لم يتم العثور على نقطة المضلع
        public const string PointNotFoundCode = "Zone.Polygon_Point_Not_Found";
        public const string PointNotFoundMessage = "Polygon point was not found.";
        // يجب أن يحتوي مضلع المنطقة على 3 نقاط على الأقل
        public const string PolygonRequiresMinimumPointsCode = "Zone.Polygon_Minimum_Points";
        public const string PolygonRequiresMinimumPointsMessage = "Zone polygon must contain at least 3 points.";
        // لا يمكن وضع علامة على المنطقة غير النشطة كخدمة
        public const string InactiveZoneCannotBeServiceableCode = "Zone.Inactive_Cannot_Be_Serviceable";
        public const string InactiveZoneCannotBeServiceableMessage = "Inactive zone cannot be marked as serviceable.";
    }
}

