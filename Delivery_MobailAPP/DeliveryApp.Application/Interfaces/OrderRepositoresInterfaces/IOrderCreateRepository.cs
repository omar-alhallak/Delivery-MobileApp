using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces
{
    public interface IOrderCreateRepository // عقد الإنشاء الذي يعزل Application عن Entity Framework
    {
        Task AddOrderAsync(Order order, CancellationToken ct = default); // تجهيز الطلب للإضافة
        Task SaveChangesAsync(CancellationToken ct = default); // حفظ التغييرات فعلياً
    }
}