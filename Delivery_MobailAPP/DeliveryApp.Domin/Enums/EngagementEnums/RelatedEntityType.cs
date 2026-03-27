namespace DeliveryApp.Domain.Enums.EngagementEnums
{
    public enum RelatedEntityType : byte // جهات التي ترسل رسائل منها
    {
        Order = 1,              // طلبات
        Complaint = 2,          // شكاوي
        AccountWarning = 3,     // تحذيرات
        DriverRequest = 4       // طلبات تقديم ك سائق
    }
}