using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

using DeliveryApp.Application.Features.Orders.Access;
using DeliveryApp.Application.Features.Notifications;

namespace DeliveryApp.Application.Features.Orders.MerchantDecision
{
    public sealed class MerchantDecisionService // Use case يجمع قرارات المطعم: قبول أو رفض
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل الطلب
        private readonly OrderAccessService _accessService;
        private readonly NotificationService _notificationService;

        public MerchantDecisionService(
            IOrderCommandRepository repository,
            OrderAccessService accessService,
            NotificationService notificationService)
        {
            _repository = repository;
            _accessService = accessService;
            _notificationService = notificationService;
        }

        public async Task<bool> ApproveAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // موافقة المطعم على الطلب
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct); // جلب الطلب
            if (order is null) return false;

            await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);

            order.ApproveByMerchant(DateTimeOffset.UtcNow); // تنفيذ الموافقة داخل الدومين
            await _notificationService.AddOrderAcceptedAsync(order, ct);

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> RejectAsync(Guid currentUserId, Guid id, MerchantDecisionRequest request, CancellationToken ct = default) // رفض المطعم للطلب
        {
            var input = MerchantDecisionValidator.ValidateReject(request); // فحص سبب الرفض
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct); // جلب الطلب

            if (order is null) return false;

            await _accessService.EnsureMerchantCanManageAsync(currentUserId, order, ct);

            order.RejectByMerchant(input.IssueReason, input.IssueNote); // تنفيذ الرفض داخل الدومين
            await _notificationService.AddOrderRejectedAsync(order, ct);

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
