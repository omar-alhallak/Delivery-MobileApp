namespace DeliveryApp.Domain.Enums.ModerationEnums.AccountWarningEnums
{
    public enum WarningDecision : byte // حالة التحذير
    {
        Pending = 1,    // معلق
        Confirmed = 2,  // تحذير صحيح
        Dismissed = 3   // تحذير غير صحيح
    }
}