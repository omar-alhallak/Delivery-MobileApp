using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.Common
{
    public sealed class OrderDto // DTO النهائي الذي يرجع للواجهة عند عرض الطلب
    {
        public Guid Id { get; init; } // معرف الطلب الداخلي
        public string? PublicId { get; init; } // رقم الطلب الظاهر للمستخدم
        public OrderType OrderType { get; init; } // نوع الطلب
        public Guid CustomerId { get; init; } // صاحب الطلب
        public Guid? MerchantId { get; init; } // المطعم المرتبط بالطلب
        public double PickupLatitude { get; init; } // إحداثيات الاستلام
        public double PickupLongitude { get; init; }
        public double DropoffLatitude { get; init; } // إحداثيات التسليم
        public double DropoffLongitude { get; init; }
        public double DistanceKm { get; init; } // المسافة المحفوظة وقت الطلب
        public decimal ItemsTotal { get; init; } // مجموع المنتجات
        public decimal DeliveryFee { get; init; } // أجرة التوصيل
        public decimal TipAmount { get; init; } // الإكرامية
        public decimal TotalAmount { get; init; } // المجموع النهائي
        public PaymentMethod PaymentMethod { get; init; } // طريقة الدفع
        public PaymentStatus PaymentStatus { get; init; } // حالة الدفع
        public OrderStatus Status { get; init; } // حالة الطلب الحالية
        public int RequiredDriversCount { get; init; } // عدد السائقين المطلوبين داخل الدومين
        public OrderIssueReason IssueReason { get; init; } // سبب المشكلة أو الرفض إن وجد
        public string? IssueNote { get; init; } // ملاحظة إضافية على المشكلة
        public CancelledByType? CancelledByType { get; init; } // الجهة التي ألغت الطلب
        public Guid? CancelledById { get; init; } // معرف من ألغى الطلب
        public DateTimeOffset? CancelledAt { get; init; } // وقت الإلغاء
        public DateTimeOffset CreatedAt { get; init; } // وقت الإنشاء
        public DateTimeOffset? ConfirmedAt { get; init; } // وقت التأكيد
        public DateTimeOffset? DeliveredAt { get; init; } // وقت التسليم
        public IReadOnlyList<OrderItemDto> Items { get; init; } = []; // المنتجات داخل الطلب
        public IReadOnlyList<OrderAssignmentDto> Assignments { get; init; } = []; // تعيينات السائقين إذا وجدت
    }
}