using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Orders.OrderWorkflow
{
    public sealed class OrderWorkflowService // Use case يجمع خطوات دورة حياة الطلب بعد الإنشاء
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل حالة الطلب

        public OrderWorkflowService(IOrderCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> SubmitToMerchantAsync(Guid id, CancellationToken ct = default) // إرسال الطلب للمطعم
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            // هذه الخطوات مخفية عن الواجهة لأن الدومين الحالي يطلبها قبل انتظار موافقة المطعم
            order.StartSearchingDrivers();
            order.ConfirmDrivers();
            order.MoveToAwaitingMerchantApproval();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ReadyForPickupAsync(Guid id, CancellationToken ct = default) // المطعم جهز الطلب
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            order.MarkReadyForPickup();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> PickedUpAsync(Guid id, CancellationToken ct = default) // السائق استلم الطلب
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            order.MarkPickedUp();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> OnTheWayAsync(Guid id, CancellationToken ct = default) // الطلب أصبح بالطريق
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            order.MarkOnTheWay();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeliveredAsync(Guid id, CancellationToken ct = default) // الطلب تم تسليمه
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            order.MarkDelivered(DateTimeOffset.UtcNow);

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
