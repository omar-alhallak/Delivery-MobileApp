using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainErrors.EngagementsErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.DriverRequest;
using DeliveryApp.Domain.Enums.EngagementsEnams;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Domain.Entities.Feedback
{
    public class Notification
    {

        public NotificationID Id { get; private set; }
        public UserID UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Body { get; private set; } = string.Empty;
        public NotificationType Type { get; private set; }
        public RelatedEntityType? RelatedEntityType { get; private set; }
        public Guid? RelatedEntityId { get; private set; }
        public bool IsRead { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ReadAt { get; private set; }

        private Notification() { }

        public Notification(UserID userId, string title, string body, NotificationType type, DateTimeOffset CreatedAtUtc, Guid? relatedEntityId = null, RelatedEntityType? relatedEntityType = null)
        {
            // 1. الفحص أولاً وقبل كل شيء(
            if (userId.IsEmpty)
                throw new DomainValidationException(ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            // 2. منطق الربط (Business Logic)
            if (relatedEntityId.HasValue && !relatedEntityType.HasValue)
                throw new DomainValidationException(NotificationErrors.RelatedEntityRequiredCode, NotificationErrors.RelatedEntityRequiredMessage, nameof(relatedEntityType));

            Id = NotificationID.New();
            UserId = userId;
            Type = type;
            RelatedEntityId = relatedEntityId;
            RelatedEntityType = relatedEntityType;

            SetTitle(title);
            SetBody(body);
            CreatedAt = CreatedAtUtc;
            IsRead = false;
        }
        
        public void MarkAsRead()
        {
            if (IsRead) return;

            IsRead = true;
            ReadAt = DateTimeOffset.UtcNow;
        }
        private void SetTitle(string title)
        {
            string? normalized = Normalize(title);

            if (normalized == null)
                throw new DomainValidationException(NotificationErrors.InvalidTitleCode,NotificationErrors.InvalidTitleMessage, nameof(Title));

            if (normalized.Length > 150)
                throw new DomainValidationException(NotificationErrors.TitleTooLongCode, NotificationErrors.TitleTooLongMessage, nameof(Title));

            Title = normalized;
        }

        private void SetBody(string body)
        {
            string? normalized = string.IsNullOrWhiteSpace(body) ? null : body.Trim();

            if (normalized == null)
                throw new DomainValidationException(NotificationErrors.InvalidBodyCode, NotificationErrors.InvalidBodyMessage, nameof(Body));

            if (normalized.Length > 1000)
                throw new DomainValidationException(NotificationErrors.BodyTooLongCode, NotificationErrors.BodyTooLongMessage, nameof(Body));

            Body = normalized;
        }

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}
