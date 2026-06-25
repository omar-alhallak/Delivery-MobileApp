using DeliveryApp.Domain.Entities.Engagements;

namespace DeliveryApp.Application.Interfaces.NotificationRepositoriesInterfaces
{
    public interface INotificationCommandRepository // عقد أوامر الإشعارات والحفظ
    {
        Task<Notification?> GetByIdAsync(NotificationID notificationId, CancellationToken ct = default); // جلب إشعار لتعديله
        Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(UserID userId, CancellationToken ct = default); // جلب الإشعارات غير المقروءة
        Task AddAsync(Notification notification, CancellationToken ct = default); // إضافة إشعار جديد
        Task SaveChangesAsync(CancellationToken ct = default); // حفظ التغييرات
    }
}
