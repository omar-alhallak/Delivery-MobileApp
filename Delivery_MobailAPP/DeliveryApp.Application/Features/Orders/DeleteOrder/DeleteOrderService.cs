using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Features.Orders.DeleteOrder
{
    public sealed class DeleteOrderService // Use case حذف الطلب
    {
        private readonly IOrderCommandRepository _repository; // Repository لتنفيذ أوامر الحذف

        public DeleteOrderService(IOrderCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ExecuteAsync(Guid id, CancellationToken ct = default) // يحذف الطلب وتعييناته إن وجدت
        {
            var orderId = OrderID.From(id); // تحويل Guid إلى Strong ID
            var order = await _repository.GetByIdAsync(orderId, ct); // جلب الطلب قبل الحذف

            if (order is null) return false;

            var assignments = await _repository.GetAssignmentsByOrderIdAsync(orderId, ct); // جلب التعيينات المرتبطة حتى لا تبقى بيانات معلقة

            _repository.RemoveAssignments(assignments);
            _repository.RemoveOrder(order);
            await _repository.SaveChangesAsync(ct);

            return true;
        }
    }
}
