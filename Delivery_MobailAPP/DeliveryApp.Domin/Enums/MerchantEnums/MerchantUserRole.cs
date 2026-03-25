namespace DeliveryApp.Domain.Enums.MerchantEnums
{
    public enum MerchantUserRole : byte // صلاحيات حسابات المطاعم
    {
        Owner = 1,     // المالك
        Staff = 2,     // المسؤل عن فرع معين
    }
}