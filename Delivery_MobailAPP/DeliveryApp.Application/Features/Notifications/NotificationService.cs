using DeliveryApp.Application.Features.Notifications.Common;
using DeliveryApp.Application.Interfaces.NotificationRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Enums.EngagementEnums;

namespace DeliveryApp.Application.Features.Notifications
{
    public sealed class NotificationService // Use case خاص بإشعارات الزبون داخل النظام
    {
        private readonly INotificationReadRepository _readRepository;
        private readonly INotificationCommandRepository _commandRepository;

        public NotificationService(INotificationReadRepository readRepository, INotificationCommandRepository commandRepository)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
        }

        public async Task<IReadOnlyList<NotificationDto>> GetByUserAsync(Guid userId, CancellationToken ct = default) // جلب إشعارات مستخدم
        {
            var notifications = await _readRepository.GetByUserAsync(UserID.From(userId), ct);
            return notifications.Select(NotificationMapper.ToDto).ToList();
        }

        public async Task<bool> MarkAsReadAsync(Guid id, CancellationToken ct = default) // تعليم إشعار واحد كمقروء
        {
            var notification = await _commandRepository.GetByIdAsync(NotificationID.From(id), ct);
            if (notification is null) return false;

            notification.MarkAsRead(DateTimeOffset.UtcNow);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default) // تعليم كل إشعارات المستخدم كمقروءة
        {
            var notifications = await _commandRepository.GetUnreadByUserAsync(UserID.From(userId), ct);

            foreach (var notification in notifications)
            {
                notification.MarkAsRead(DateTimeOffset.UtcNow);
            }

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }

        public async Task AddOrderAcceptedAsync(Order order, CancellationToken ct = default) // إشعار قبول الطلب
        {
            await AddOrderNotificationAsync(order, "تم قبول طلبك", "تم قبول طلبك وهو قيد التجهيز.", ct);
        }

        public async Task AddOrderRejectedAsync(Order order, CancellationToken ct = default) // إشعار رفض الطلب
        {
            await AddOrderNotificationAsync(order, "تم رفض طلبك", "تم رفض الطلب من المطعم.", ct);
        }

        public async Task AddOrderReadyAsync(Order order, CancellationToken ct = default) // إشعار جاهزية الطلب
        {
            await AddOrderNotificationAsync(order, "طلبك جاهز للاستلام", "طلبك جاهز للاستلام.", ct);
        }

        public async Task AddOrderOnTheWayAsync(Order order, CancellationToken ct = default) // إشعار أن الطلب بالطريق
        {
            await AddOrderNotificationAsync(order, "طلبك بالطريق", "طلبك أصبح بالطريق إليك.", ct);
        }

        public async Task AddOrderDeliveredAsync(Order order, CancellationToken ct = default) // إشعار تسليم الطلب
        {
            await AddOrderNotificationAsync(order, "تم تسليم طلبك", "تم تسليم طلبك بنجاح.", ct);
        }

        private async Task AddOrderNotificationAsync(Order order, string title, string body, CancellationToken ct) // إنشاء إشعار مرتبط بطلب
        {
            var notification = new Notification(
                order.CustomerID,
                title,
                body,
                DateTimeOffset.UtcNow,
                order.ID.Value,
                RelatedEntityType.Order);

            await _commandRepository.AddAsync(notification, ct);
        }
    }
}
