using DeliveryApp.Application.Features.Orders.Common;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Application.Features.Orders.Access;
using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Features.Orders.GetOrders
{
    public sealed class OrderQueryService // Use case واحد مسؤول عن قراءة الطلبات
    {
        private readonly IOrderReadRepository _repository; // Repository خاص بالقراءة فقط
        private readonly OrderAccessService _accessService;

        public OrderQueryService(IOrderReadRepository repository, OrderAccessService accessService)
        {
            _repository = repository;
            _accessService = accessService;
        }

        public async Task<IReadOnlyList<OrderDto>> GetForCustomerAsync(Guid currentUserId, CancellationToken ct = default)
        {
            var orders = await _repository.GetByCustomerAsync(UserID.From(currentUserId), ct);
            return await MapOrdersAsync(orders, ct);
        }

        public async Task<IReadOnlyList<OrderDto>> GetForMerchantAsync(Guid currentUserId, Guid merchantId, CancellationToken ct = default)
        {
            await _accessService.EnsureMerchantCanManageAsync(currentUserId, merchantId, ct);
            var orders = await _repository.GetByMerchantAsync(MerchantID.From(merchantId), ct);
            return await MapOrdersAsync(orders, ct);
        }

        public async Task<OrderDto?> GetByIdAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // يرجع طلب واحد أو null إذا غير موجود
        {
            var orderId = OrderID.From(id); // تحويل Guid إلى Strong ID خاص بالدومين
            var order = await _repository.GetByIdAsync(orderId, ct); // جلب الطلب

            if (order is null) return null;

            await _accessService.EnsureCanReadAsync(currentUserId, order, ct);

            var assignments = await _repository.GetAssignmentsByOrderIdAsync(orderId, ct); // جلب تعيينات الطلب
            return OrderMapper.ToDto(order, assignments);
        }

        private async Task<IReadOnlyList<OrderDto>> MapOrdersAsync(IReadOnlyList<Order> orders, CancellationToken ct)
        {
            var orderIds = orders.Select(x => x.ID).ToList();
            var assignments = await _repository.GetAssignmentsByOrderIdsAsync(orderIds, ct);

            return orders
                .Select(order => OrderMapper.ToDto(order, assignments.Where(x => x.OrderID == order.ID)))
                .ToList();
        }
    }
}
