using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Infrastructure.Persistence;

namespace DeliveryApp.Infrastructure.Implementation.OrderRepositores
{
    public sealed class OrderCreateRepository : IOrderCreateRepository // تنفيذ التخزين الخاص بإنشاء الطلب
    {
        private readonly ApplicationDbContext _context; // DbContext هو بوابة التعامل مع SQL Server

        public OrderCreateRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddOrderAsync(Order order, CancellationToken ct = default) // إضافة الطلب إلى Change Tracker
        {
            await _context.Orders.AddAsync(order, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default) // تنفيذ INSERT فعلياً بقاعدة البيانات
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
