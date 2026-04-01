namespace DeliveryApp.Domain.Enums.OrderEnums
{
    public enum CancelledByType : byte // جهة الي رفضت الطلب
    {
        Customer = 1,   // زبون
        Driver = 2,     // سائق
        Merchant = 3,   // تاجر
        Admin = 4       // المشرف
    }
}