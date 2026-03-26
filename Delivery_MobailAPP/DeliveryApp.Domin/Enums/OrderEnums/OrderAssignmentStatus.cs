namespace DeliveryApp.Domain.Enums.OrderEnums
{
    public enum OrderAssignmentStatus : byte // حالة تعيين سائق للطلب
    {
        Pending = 1,    // معلق
        Accepted = 2,   // تم قبوله
        Rejected = 3,   // تم رفضه
        Removed = 4     // تم إلغاءه
    }
}