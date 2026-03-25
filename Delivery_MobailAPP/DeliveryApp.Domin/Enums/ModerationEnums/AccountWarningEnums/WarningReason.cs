namespace DeliveryApp.Domain.Enums.ModerationEnums.AccountWarningEnums
{
    public enum WarningReason : byte // سبب التحذير
    {
        // Customer // 1 > 9
        FakeOrder = 1,                // طلب وهمي
        CustomerNoPayment = 2,        // رفض الدفع
        CustomerAbuse = 3,            // إساءة للسائق أو التاجر
        CustomerNoShow = 4,           // لم يستلم الطلب
        WrongAddress = 5,             // عنوان خاطأ

        // Driver // 10 > 19
        LateDelivery = 10,            // تأخير التوصيل
        DriverAbuse = 11,             // إساءة للزبون
        OrderTheft = 12,              // سرقة الطلب
        DriverOrderAbandonment = 13,  // رفض الطلب بعد قبوله

        // Merchant // 20 > 29
        MerchantAbuse = 20,           // إساءة للسائق
        OrderManipulation = 21,       // تلاعب بالطلب
        IncompleteOrder = 22,         // طلب ناقص
        WrongOrderPrepared = 23,      // طلب خاطئ

        // General // 30 > 39
        AbusiveLanguage = 30,         // ألفاظ مسيئة
        Fraud = 31,                   // احتيال عام
        PolicyViolation = 32,         // مخالفة سياسة عامة
        Other = 39                    // سبب آخر
    }
}