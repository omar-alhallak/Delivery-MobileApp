using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Order
{
    public enum OrderStatus : short
    {
        Draft = 0,                       // قبل التأكيد
        SearchingDrivers = 10,           // البحث عن سائقين
        DriversConfirmed = 20,           // تم قبول الطلب من قبل السائق أو سائقين إذا موزع 

        AwaitingMerchantApproval = 30,   // بانتظار قبول تاجر
        Preparing = 40,                  // تحضير الطلب 
        ReadyForPickup = 50,             // جاهز للاستلام

        PickedUp = 60,                   // السائق استلم
        OnTheWay = 70,                   // على الطريق 
        Delivered = 90,                  // تم التسليم

        Cancelled = 95,                  // ملغي
        DeliveryFailed = 96              // الغي لسبب من قبل المشرف
    }
}