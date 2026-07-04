using DeliveryApp.Domain.Entities.Engagements;

namespace DeliveryApp.Application.Interfaces.NotificationRepositoriesInterfaces
{
    public interface INotificationReadRepository // عقد قراءة الإشعارات بدون معرفة تفاصيل قاعدة البيانات
    {
        Task<IReadOnlyList<Notification>> GetByUserAsync(UserID userId, CancellationToken ct = default); // جلب إشعارات مستخدم
    }
}