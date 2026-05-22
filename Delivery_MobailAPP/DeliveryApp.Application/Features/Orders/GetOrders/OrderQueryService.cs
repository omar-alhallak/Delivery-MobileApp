using DeliveryApp.Application.Features.Orders.Common;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Orders.GetOrders
{
    public sealed class OrderQueryService // Use case واحد مسؤول عن قراءة الطلبات
    {
        private readonly IOrderReadRepository _repository; // Repository خاص بالقراءة فقط

        public OrderQueryService(IOrderReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken ct = default) // يرجع كل الطلبات للواجهة
        {
            var orders = await _repository.GetAllAsync(ct); // جلب الطلبات
            var assignments = await _repository.GetAllAssignmentsAsync(ct); // جلب التعيينات وربطها مع الطلبات

            return orders
                .Select(order => OrderMapper.ToDto(order, assignments.Where(x => x.OrderID == order.ID)))
                .ToList();
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default) // يرجع طلب واحد أو null إذا غير موجود
        {
            var orderId = OrderID.From(id); // تحويل Guid إلى Strong ID خاص بالدومين
            var order = await _repository.GetByIdAsync(orderId, ct); // جلب الطلب

            if (order is null) return null;

            var assignments = await _repository.GetAssignmentsByOrderIdAsync(orderId, ct); // جلب تعيينات الطلب
            return OrderMapper.ToDto(order, assignments);
        }
    }
}
