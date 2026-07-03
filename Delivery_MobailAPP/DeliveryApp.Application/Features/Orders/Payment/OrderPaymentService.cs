using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

using DeliveryApp.Application.Features.Orders.Access;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.Payment
{
    public sealed class OrderPaymentService // Use case يجمع تعديل حالة الدفع
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل الطلب
        private readonly OrderAccessService _accessService;

        public OrderPaymentService(IOrderCommandRepository repository, OrderAccessService accessService)
        {
            _repository = repository;
            _accessService = accessService;
        }

        public async Task<bool> MarkAsPaidAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // تعليم الطلب كمدفوع
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);

            if (order.Status != OrderStatus.Delivered)
                throw new DomainRuleViolationException("Order.Payment_Before_Delivery", "Cash order can be marked as paid only after delivery.");

            order.MarkAsPaid();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> MarkAsUnpaidAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // تعليم الطلب كغير مدفوع
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);

            order.MarkAsUnpaid();

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
