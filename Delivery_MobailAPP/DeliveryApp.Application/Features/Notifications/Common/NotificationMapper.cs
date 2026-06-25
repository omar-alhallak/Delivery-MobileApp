using DeliveryApp.Domain.Entities.Engagements;

namespace DeliveryApp.Application.Features.Notifications.Common
{
    public static class NotificationMapper // يحول الإشعار من Entity داخلي إلى DTO للفرونت
    {
        public static NotificationDto ToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.ID.Value,
                UserId = notification.UserID.Value,
                Title = notification.Title,
                Body = notification.Body,
                RelatedEntityType = notification.RelatedEntityType.HasValue ? (int)notification.RelatedEntityType.Value : null,
                RelatedEntityId = notification.RelatedEntityID,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };
        }
    }
}
