namespace DeliveryApp.Domain.Enums.DriverEnums
{
    public enum DriverRequestStatus : byte // حالة طلب التقديم ك سائق
    {
        Pending = 1,    // معلق
        Approved = 2,   // مقبول
        Rejected = 3    // مرفوض
    }
}