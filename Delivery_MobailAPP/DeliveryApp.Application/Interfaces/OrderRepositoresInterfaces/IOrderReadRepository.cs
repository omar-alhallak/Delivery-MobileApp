using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces
{
    public interface IOrderReadRepository // عقد القراءة الذي تحتاجه Application بدون معرفة تفاصيل قاعدة البيانات
    {
        Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default); // جلب كل الطلبات
        Task<Order?> GetByIdAsync(OrderID orderId, CancellationToken ct = default); // جلب طلب واحد
        Task<IReadOnlyList<OrderAssignment>> GetAssignmentsByOrderIdAsync(OrderID orderId, CancellationToken ct = default); // جلب تعيينات طلب محدد
        Task<IReadOnlyList<OrderAssignment>> GetAllAssignmentsAsync(CancellationToken ct = default); // جلب كل التعيينات لاستخدامها مع قائمة الطلبات
    }
}
