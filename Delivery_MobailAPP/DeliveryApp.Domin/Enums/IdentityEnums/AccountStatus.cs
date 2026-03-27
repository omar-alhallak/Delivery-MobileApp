namespace DeliveryApp.Domain.Enums.IdentityEnums
{
    public enum AccountStatus : byte // حالات الحساب
    {
        Active = 1,      // نشط
        Suspended = 2,   // معلق(مؤقتاَ ,دائماَ)د 
        Banned = 3,      // محظور
    }
}