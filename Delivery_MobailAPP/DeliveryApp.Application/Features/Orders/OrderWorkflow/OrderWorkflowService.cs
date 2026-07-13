using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

using DeliveryApp.Application.Features.Orders.Access;
using DeliveryApp.Application.Features.Notifications;

namespace DeliveryApp.Application.Features.Orders.OrderWorkflow
{
    public sealed class OrderWorkflowService // Use case يجمع خطوات دورة حياة الطلب بعد الإنشاء
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل حالة الطلب
        private readonly OrderAccessService _accessService;
        private readonly NotificationService _notificationService;

        public OrderWorkflowService(
            IOrderCommandRepository repository,
            OrderAccessService accessService,
            NotificationService notificationService)
        {
            _repository = repository;
            _accessService = accessService;
            _notificationService = notificationService;
        }

        public async Task<bool> SubmitToMerchantAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // إرسال الطلب للمطعم
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            if (order.CustomerID.Value != currentUserId)
                throw new UnauthorizedAccessException("Customer order access denied.");

            // هذه الخطوات مخفية عن الواجهة لأن الدومين الحالي يطلبها قبل انتظار موافقة المطعم
            order.StartSearchingDrivers();
            order.ConfirmDrivers();
            order.MoveToAwaitingMerchantApproval();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ReadyForPickupAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // المطعم جهز الطلب
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);

            order.MarkReadyForPickup();
            await _notificationService.AddOrderReadyAsync(order, ct);

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> PickedUpAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // السائق استلم الطلب
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);

            order.MarkPickedUp();
            order.MarkOnTheWay();
            await _notificationService.AddOrderOnTheWayAsync(order, ct);

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> OnTheWayAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // الطلب أصبح بالطريق
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);

            order.MarkOnTheWay();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeliveredAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // الطلب تم تسليمه
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            //await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);
            if (order.CustomerID.Value != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the order customer can confirm delivery.");
            }

            order.MarkDelivered(DateTimeOffset.UtcNow);
            order.MarkAsPaid();
            await _notificationService.AddOrderDeliveredAsync(order, ct);

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
