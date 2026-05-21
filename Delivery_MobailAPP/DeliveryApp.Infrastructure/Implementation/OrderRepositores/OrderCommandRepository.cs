using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OrderID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderTag>;

namespace DeliveryApp.Infrastructure.Implementation.OrderRepositores
{
    public sealed class OrderCommandRepository : IOrderCommandRepository // تنفيذ أوامر تعديل وحذف الطلب
    {
        private readonly ApplicationDbContext _context; // DbContext الذي يتابع التغييرات ويحفظها

        public OrderCommandRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order?> GetByIdAsync(OrderID orderId, CancellationToken ct = default) // جلب الطلب للتعديل عليه
        {
            return await _context.Orders.FindAsync([orderId], ct);
        }

        public async Task<IReadOnlyList<OrderAssignment>> GetAssignmentsByOrderIdAsync(OrderID orderId, CancellationToken ct = default) // جلب تعيينات الطلب قبل الحذف
        {
            return await _context.OrderAssignments
                .Where(x => x.OrderID == orderId)
                .ToListAsync(ct);
        }

        public void RemoveOrder(Order order) // تعليم الطلب للحذف
        {
            _context.Orders.Remove(order);
        }

        public void RemoveAssignments(IEnumerable<OrderAssignment> assignments) // تعليم تعيينات السائقين للحذف
        {
            _context.OrderAssignments.RemoveRange(assignments);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default) // تنفيذ التغييرات فعلياً في قاعدة البيانات
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
