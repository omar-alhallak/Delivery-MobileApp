using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

using DeliveryApp.Application.Features.Orders.Access;

namespace DeliveryApp.Application.Features.Orders.CancelOrder
{
    public sealed class CancelOrderService // Use case إلغاء الطلب
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل الطلب
        private readonly OrderAccessService _accessService;

        public CancelOrderService(IOrderCommandRepository repository, OrderAccessService accessService)
        {
            _repository = repository;
            _accessService = accessService;
        }

        public async Task<bool> ExecuteAsync(Guid currentUserId, Guid id, CancelOrderRequest request, CancellationToken ct = default) // يحفظ حالة الإلغاء وسببه
        {
            var input = CancelOrderValidator.Validate(request); // فحص بيانات الإلغاء
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct); // جلب الطلب

            if (order is null) return false;

            var cancelledByType = await _accessService.GetCancellationActorAsync(currentUserId, order, ct);
            CancelOrderValidator.ValidateReason(input, cancelledByType);

            // تنفيذ الإلغاء داخل الدومين حتى تبقى قواعد الحالة بمكانها الصحيح
            order.Cancel(
                cancelledByType,
                UserID.From(currentUserId),
                input.IssueReason,
                input.IssueNote,
                DateTimeOffset.UtcNow);

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
