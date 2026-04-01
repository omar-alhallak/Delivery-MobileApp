namespace DeliveryApp.Domain.Enums.IdentityEnums
{
    public enum ClientType : byte // جهة التي دخل منها الحساب للنظام  
    {
        CustomerApp = 1,      // تطبيق الزبون  (MobileApp)
        DriverApp = 2,        // تطبيق السائق  (MobileApp)
        MerchantWebApp = 3,   // تطبيق المطعم  (Wep App)
        AdminDashboard = 4    // لوحة المشرفين (Dashboard)
    }
}