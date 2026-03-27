namespace DeliveryApp.Domain.Enums.OrderEnums
{
    public enum PaymentStatus : byte // حالة الدفع
    {
        UnPaid = 0,   // لم يتم الدفع
        Paid = 1,     // تم الدفع
    }
}