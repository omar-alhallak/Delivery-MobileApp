using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OrderID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderTag>;

namespace DeliveryApp.Infrastructure.Implementation.OrderRepositores
{
    public sealed class OrderReadRepository : IOrderReadRepository // تنفيذ قراءة الطلبات من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context; // DbContext الخاص بـ Entity Framework

        public OrderReadRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default) // جلب كل الطلبات مع المنتجات
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<Order?> GetByIdAsync(OrderID orderId, CancellationToken ct = default) // جلب طلب واحد مع المنتجات
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.ID == orderId, ct);
        }

        public async Task<IReadOnlyList<OrderAssignment>> GetAssignmentsByOrderIdAsync(OrderID orderId, CancellationToken ct = default) // جلب تعيينات طلب محدد
        {
            return await _context.OrderAssignments
                .AsNoTracking()
                .Where(x => x.OrderID == orderId)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<OrderAssignment>> GetAllAssignmentsAsync(CancellationToken ct = default) // جلب كل التعيينات للعرض مع قائمة الطلبات
        {
            return await _context.OrderAssignments
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
