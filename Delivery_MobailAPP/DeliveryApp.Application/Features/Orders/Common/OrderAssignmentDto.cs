using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.Common
{
    public sealed class OrderAssignmentDto // DTO يعرض ربط الطلب مع السائق إن وجد
    {
        public Guid Id { get; init; } // معرف التعيين
        public Guid OrderId { get; init; } // الطلب المرتبط
        public Guid DriverId { get; init; } // السائق المرتبط
        public DateTimeOffset AssignedAt { get; init; } // وقت التعيين
        public OrderAssignmentStatus Status { get; init; } // حالة التعيين
        public DateTimeOffset? RemovedAt { get; init; } // وقت إزالة التعيين
        public string? RemoveReason { get; init; } // سبب الإزالة
    }
}
