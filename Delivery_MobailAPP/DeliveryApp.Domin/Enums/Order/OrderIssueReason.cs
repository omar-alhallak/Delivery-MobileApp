using System;

namespace DeliveryApp.Domain.Enums.Order
{
    public enum OrderIssueReason : byte
    {
        None = 0,

        // Merchant // 1 > 9
        VendorBusy = 1,            // المطعم مشغول
        VendorClosed = 2,          // المطعم مغلق
        OutOfStock = 3,            // عنصر غير متوفر
        VendorRejectedOther = 9,   // سبب آخر من المطعم

        // Driver // 10 > 19
        DriverAccident = 10,        // حادث / صارلو شي
        VehicleBreakdown = 11,      // عطل بالمركبة
        DriverNoShow = 12,          // السائق ما حضر/اختفى
        DriverRejected = 13,        // السائق رفض بعد قبول
        DeliveryIssueOther = 19,    // سبب آخر من السائق

        // Customer // 20 > 29
        CustomerNoAnswer = 20,      // ما بيرد
        CustomerNotAvailable = 21,  // موجود بس ما فتح / ما استلم
        WrongAddress = 22,          // العنوان غلط
        CustomerInsistedCancel = 23,// مصر يلغي لظرف طارء
        CustomerIssueOther = 24,    // سبب آخر من زبون

        // Admin // 30 > 39
        NoDriversAvailable = 30,    // ما لقينا سائقين
        AdminDecision = 31,         // قرار إداري
        SystemError = 32,           // مشكلة تقنية
        Other = 39                  // سبب آخر من المشرف
    }
}