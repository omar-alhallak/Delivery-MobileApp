using DeliveryApp.Application.Interfaces.NotificationRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.NotificationRepositores
{
    public sealed class NotificationReadRepository : INotificationReadRepository // تنفيذ قراءة الإشعارات من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context;

        public NotificationReadRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<Notification>> GetByUserAsync(UserID userId, CancellationToken ct = default) // جلب إشعارات مستخدم مرتبة من الأحدث للأقدم
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserID == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
