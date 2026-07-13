namespace DeliveryApp.Domain.Enums.OrderEnums
{
    public enum OrderStatus : byte // حالة الطلب
    {
        Draft = 1,                       // قبل التأكيد
        SearchingDrivers = 2,           // البحث عن سائقين
        DriversConfirmed = 3,           // تم قبول الطلب من قبل السائق أو سائقين إذا موزع 

        AwaitingMerchantApproval = 4,   // بانتظار قبول تاجر
        Preparing = 5,                  // تحضير الطلب 
        ReadyForPickup = 6,             // جاهز للاستلام

        PickedUp = 7,                   // السائق استلم
        OnTheWay = 8,                   // على الطريق 
        Delivered = 9,                  // تم التسليم

        Cancelled = 10,                  // ملغي
        DeliveryFailed = 11             // الغي لسبب من قبل المشرف          
    }
}