using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public sealed class CreateOrderRequest // DTO يستقبل بيانات إنشاء الطلب من الواجهة
    {
        public OrderType OrderType { get; init; } // نوع الطلب
        public Guid CustomerId { get; init; } // معرف المستخدم صاحب الطلب
        public Guid? MerchantId { get; init; } // معرف المطعم إذا الطلب مرتبط بمطعم
        public double PickupLatitude { get; init; } // خط عرض نقطة الاستلام
        public double PickupLongitude { get; init; } // خط طول نقطة الاستلام
        public double DropoffLatitude { get; init; } // خط عرض نقطة التسليم
        public double DropoffLongitude { get; init; } // خط طول نقطة التسليم
        public double DistanceKm { get; init; } // المسافة المحسوبة لحظة إنشاء الطلب
        public decimal ItemsTotal { get; init; } // مجموع أسعار المنتجات فقط
        public decimal DeliveryFee { get; init; } // أجرة التوصيل
        public decimal TipAmount { get; init; } // الإكرامية إن وجدت
        public PaymentMethod PaymentMethod { get; init; } // طريقة الدفع
        public PaymentStatus PaymentStatus { get; init; } // حالة الدفع عند الإنشاء
        public int RequiredDriversCount { get; init; } // عدد السائقين المطلوبين حسب قواعد الدومين
        public IReadOnlyList<OrderItemRequest> Items { get; init; } = []; // المنتجات المطلوبة
    }
}