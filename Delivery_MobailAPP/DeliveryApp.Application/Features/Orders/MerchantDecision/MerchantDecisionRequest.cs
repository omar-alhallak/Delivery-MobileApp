using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.MerchantDecision
{
    public sealed class MerchantDecisionRequest // DTO يستخدم عند رفض المطعم للطلب
    {
        public OrderIssueReason IssueReason { get; init; } // سبب الرفض
        public string? IssueNote { get; init; } // ملاحظة توضيحية اختيارية
    }
}
