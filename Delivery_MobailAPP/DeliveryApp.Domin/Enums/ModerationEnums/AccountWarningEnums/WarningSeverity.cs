namespace DeliveryApp.Domain.Enums.ModerationEnums.AccountWarningEnums
{
    public enum WarningSeverity : byte // درجة خطورة التحذير
    {
        Low = 1,        // منخفضة
        Medium = 2,     // متوسطة
        High = 3,       // عالية
        Critical = 4    // مخالفة خطيرة جداَ تحتاج إجراءات فورية
    }
}