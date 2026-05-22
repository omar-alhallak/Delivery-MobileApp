using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Orders.MerchantDecision
{
    public sealed class MerchantDecisionService // Use case يجمع قرارات المطعم: قبول أو رفض
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل الطلب

        public MerchantDecisionService(IOrderCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ApproveAsync(Guid id, CancellationToken ct = default) // موافقة المطعم على الطلب
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct); // جلب الطلب
            if (order is null) return false;

            order.ApproveByMerchant(DateTimeOffset.UtcNow); // تنفيذ الموافقة داخل الدومين

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> RejectAsync(Guid id, MerchantDecisionRequest request, CancellationToken ct = default) // رفض المطعم للطلب
        {
            var input = MerchantDecisionValidator.ValidateReject(request); // فحص سبب الرفض
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct); // جلب الطلب

            if (order is null) return false;

            order.RejectByMerchant(input.IssueReason, input.IssueNote); // تنفيذ الرفض داخل الدومين

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
