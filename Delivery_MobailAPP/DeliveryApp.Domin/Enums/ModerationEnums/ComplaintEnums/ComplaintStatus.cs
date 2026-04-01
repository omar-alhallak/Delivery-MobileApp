namespace DeliveryApp.Domain.Enums.ModerationEnums.ComplaintEnums
{
    public enum ComplaintStatus : byte // حالة الشكوة
    {
        Pending = 1,    // معلقة
        Resolved = 2,   // شكوة صحيصة
        Rejected = 3    // شكوة غير صحيصة 
    }
}