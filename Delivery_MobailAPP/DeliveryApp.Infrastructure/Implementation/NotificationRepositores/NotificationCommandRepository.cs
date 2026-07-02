using DeliveryApp.Application.Interfaces.NotificationRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using NotificationID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.NotificationTag>;

namespace DeliveryApp.Infrastructure.Implementation.NotificationRepositores
{
    public sealed class NotificationCommandRepository : INotificationCommandRepository // تنفيذ أوامر الإشعارات من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context;

        public NotificationCommandRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Notification?> GetByIdAsync(NotificationID notificationId, CancellationToken ct = default)
            => await _context.Notifications.FirstOrDefaultAsync(x => x.ID == notificationId, ct);

        public async Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.Notifications
                .Where(x => x.UserID == userId && !x.IsRead)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
            => await _context.Notifications.AddAsync(notification, ct);

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
