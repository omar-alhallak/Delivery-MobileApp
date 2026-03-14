using System;

namespace DeliveryApp.Domain.Enums.ComplaintEnums
{
    public enum ComplaintReason : byte
    {
        // Service - Delivery // 1 > 9
        LateDelivery = 1,                // تأخير في التوصيل
        OrderNotDelivered = 2,           // الطلب لم يصل
        DriverNoShow = 3,                // السائق لم يحضر

        // Order - Item // 10 > 19
        WrongOrder = 10,                 // الطلب خاطئ
        MissingItems = 11,               // عناصر ناقصة
        DamagedOrder = 12,               // الطلب تالف
        OrderQualityIssue = 13,          // جودة الطلب سيئة
        IncorrectItemPreparation = 14,   // تحضير الطلب بطريقة خاطئة
        PackagingIssue = 15,             // تغليف سيء أدى لتلف الطلب

        // Payment // 20 > 29
        PaymentDispute = 20,             // نزاع دفع
        RefusedToPay = 21,               // رفض الدفع

        // Conduct // 30 > 39
        AbusiveBehavior = 30,           // سلوك مسيء
        UnprofessionalBehavior = 31,    // سلوك غير مهني
        Fraud = 32,                     // احتيال
        ThreateningBehavior = 33,       // تهديد

        // General - Policy // 40 > 49
        PolicyViolation = 40,           // مخالفة سياسة
        SpamComplaint = 41,             // شكوة كاذبة أو متكررة
        Other = 49                      // سبب آخر
    }
}