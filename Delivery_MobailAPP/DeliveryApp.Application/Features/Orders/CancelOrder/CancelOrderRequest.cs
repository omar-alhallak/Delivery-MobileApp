using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.CancelOrder
{
    public sealed class CancelOrderRequest // DTO يستقبل بيانات إلغاء الطلب
    {
        public CancelledByType CancelledByType { get; init; } // نوع الجهة التي ألغت الطلب
        public Guid CancelledById { get; init; } // معرف الشخص أو الجهة التي ألغت
        public OrderIssueReason IssueReason { get; init; } // سبب الإلغاء
        public string? IssueNote { get; init; } // ملاحظة إضافية اختيارية
    }
}
