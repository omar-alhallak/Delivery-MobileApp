using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Orders.Payment
{
    public sealed class OrderPaymentService // Use case يجمع تعديل حالة الدفع
    {
        private readonly IOrderCommandRepository _repository; // Repository لتعديل الطلب

        public OrderPaymentService(IOrderCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> MarkAsPaidAsync(Guid id, CancellationToken ct = default) // تعليم الطلب كمدفوع
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            order.MarkAsPaid();

            await _repository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> MarkAsUnpaidAsync(Guid id, CancellationToken ct = default) // تعليم الطلب كغير مدفوع
        {
            var order = await _repository.GetByIdAsync(OrderID.From(id), ct);
            if (order is null) return false;

            order.MarkAsUnpaid();

            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}