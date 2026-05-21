using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces
{
    public interface IOrderCommandRepository // عقد الأوامر: تعديل أو حذف الطلبات
    {
        Task<Order?> GetByIdAsync(OrderID orderId, CancellationToken ct = default); // جلب الطلب حتى نغير حالته
        Task<IReadOnlyList<OrderAssignment>> GetAssignmentsByOrderIdAsync(OrderID orderId, CancellationToken ct = default); // جلب التعيينات المرتبطة
        void RemoveOrder(Order order); // تعليم الطلب للحذف
        void RemoveAssignments(IEnumerable<OrderAssignment> assignments); // تعليم التعيينات للحذف
        Task SaveChangesAsync(CancellationToken ct = default); // حفظ التعديلات
    }
}
