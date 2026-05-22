using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Features.Orders.CancelOrder
{
    public sealed class CancelOrderService // Use case إلغاء الطلب
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل الطلب

        public CancelOrderService(IOrderCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ExecuteAsync(Guid id, CancelOrderRequest request, CancellationToken ct = default) // يحفظ حالة الإلغاء وسببه
        {
            var input = CancelOrderValidator.Validate(request); // فحص بيانات الإلغاء
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct); // جلب الطلب

            if (order is null) return false;

            // تنفيذ الإلغاء داخل الدومين حتى تبقى قواعد الحالة بمكانها الصحيح
            order.Cancel(
                input.CancelledByType,
                UserID.From(input.CancelledById),
                input.IssueReason,
                input.IssueNote,
                DateTimeOffset.UtcNow);

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
