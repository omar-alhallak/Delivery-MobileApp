using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces
{
    public interface IOrderReadRepository // عقد القراءة الذي تحتاجه Application بدون معرفة تفاصيل قاعدة البيانات
    {
        Task<IReadOnlyList<Order>> GetByCustomerAsync(UserID customerId, CancellationToken ct = default);
        Task<IReadOnlyList<Order>> GetByMerchantAsync(MerchantID merchantId, CancellationToken ct = default);
        Task<Order?> GetByIdAsync(OrderID orderId, CancellationToken ct = default); // جلب طلب واحد
        Task<IReadOnlyList<OrderAssignment>> GetAssignmentsByOrderIdAsync(OrderID orderId, CancellationToken ct = default); // جلب تعيينات طلب محدد
        Task<IReadOnlyList<OrderAssignment>> GetAssignmentsByOrderIdsAsync(IReadOnlyCollection<OrderID> orderIds, CancellationToken ct = default);
    }
}