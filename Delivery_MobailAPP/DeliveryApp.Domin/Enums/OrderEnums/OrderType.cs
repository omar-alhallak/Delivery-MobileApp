namespace DeliveryApp.Domain.Enums.OrderEnums
{
    public enum OrderType : byte // نوع الطلب
    {
        Near = 1,       // قريب
        Far = 2,        // بعيد
        Shipping = 3    // شحن
    }
}