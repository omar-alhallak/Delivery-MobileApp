using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Enums.Order
{
    public enum OrderIssueReason : byte
    {
        None = 0,

        // Merchant
        VendorBusy = 10,            // المطعم مشغول
        VendorClosed = 11,          // المطعم مغلق
        OutOfStock = 12,            // عنصر غير متوفر
        VendorRejectedOther = 19,   // سبب آخر من المطعم

        // Driver
        DriverAccident = 30,        // حادث / صارلو شي
        VehicleBreakdown = 31,      // عطل بالمركبة
        DriverNoShow = 32,          // السائق ما حضر/اختفى
        DriverRejected = 33,        // السائق رفض بعد قبول
        DeliveryIssueOther = 39,    // سبب آخر من السائق

        // Customer
        CustomerNoAnswer = 50,      // ما بيرد
        CustomerNotAvailable = 51,  // موجود بس ما فتح / ما استلم
        WrongAddress = 52,          // العنوان غلط
        CustomerInsistedCancel = 53,// مصر يلغي لظرف طارء
        CustomerIssueOther = 59,    // سبب آخر من زبون

        // Admin
        NoDriversAvailable = 70,    // ما لقينا سائقين
        AdminDecision = 71,         // قرار إداري
        SystemError = 72,           // مشكلة تقنية
        Other = 99                  // سبب آخر من المشرف
    }
}