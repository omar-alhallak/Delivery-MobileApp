namespace DeliveryApp.Application.Features.Notifications.Common
{
    public sealed class NotificationDto // البيانات التي تظهر للفرونت عند جلب الإشعارات
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public int? RelatedEntityType { get; init; }
        public Guid? RelatedEntityId { get; init; }
        public bool IsRead { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? ReadAt { get; init; }
    }
}
