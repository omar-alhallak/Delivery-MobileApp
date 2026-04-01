namespace DeliveryApp.Domain.Enums.ModerationEnums.AccountWarningEnums
{
    public enum WarningEntityType : byte // جهة التي يتم تحذيرها
    {
        Customer = 1,   // زبون
        Driver = 2,     // سائق
        Merchant = 3    // تاجر
    }
}