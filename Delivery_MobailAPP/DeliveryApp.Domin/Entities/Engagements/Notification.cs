using DeliveryApp.Domain.DomainErrors.EngagementsErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.DriverRequest;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.EngagementsEnams;

namespace DeliveryApp.Domain.Entities.Feedback
{
    public class Notification
    {
        public Guid NotificationID { get; private set; }

        public Guid UserId { get; private set; }
        public User? User { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public string Body { get; private set; } = string.Empty;

        public int Type { get; private set; }

        public RelatedEntityType? RelatedEntityType { get; private set; }

        public Guid? RelatedEntityID { get; private set; }
        public User? RelatedEntity { get; private set; }

        public bool IsRead { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Notification() { }
        public Notification(Guid userId, string title, string body, int type, RelatedEntityType? relatedEntityType = null, Guid? relatedEntityId = null)
        {
            NotificationID = Guid.NewGuid();
            UserId = userId;
            Title = Normalize(title);
            Body = Normalize(body);
            Type = type;
            RelatedEntityType = relatedEntityType;
            RelatedEntityID = relatedEntityId;
            IsRead = false;
        }
        public void MarkAsRead()
        {
            if (IsRead) return;

            IsRead = true;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        private void SetTitle(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException(NotificationErrors.InvalidTitleCode, NotificationErrors.InvalidTitleMessage, nameof(Title));

            value = value.Trim();

            if (value.Length > 150)
                throw new DomainValidationException(NotificationErrors.TooLongCode, NotificationErrors.TooLongMessage, nameof(Title));

            Title = value;
        }
        private void SetBody(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException(NotificationErrors.InvalidBodyCode, NotificationErrors.InvalidBodyMessage, nameof(Body));

            value = value.Trim();

            if (value.Length > 1000)
                throw new DomainValidationException(NotificationErrors.TooLongCode, NotificationErrors.TooLongMessage, nameof(Body));

            Body = value;
        }
        public void UpdateContent(string newTitle, string newBody)
        {
            if (string.IsNullOrWhiteSpace(newTitle) || newTitle.Length < 3 || newTitle.Length > 150)
            {
                throw new DomainValidationException(NotificationErrors.InvalidTitleCode, NotificationErrors.InvalidTitleMessage, nameof(newTitle));
            }
            SetTitle(newTitle);
            SetBody(newBody);
        }
        public void UpdateRelatedEntity(RelatedEntityType? newType, Guid? newId)
        {
            if (newType.HasValue && !newId.HasValue)
            {
                throw new DomainRuleViolationException(NotificationErrors.RelatedEntityRequiredCode, NotificationErrors.RelatedEntityRequiredMessage);
            }

            if (newType.HasValue) RelatedEntityType = newType.Value;

            if (newId.HasValue) RelatedEntityID = newId.Value;
        }

        public void AttachOrder(Guid orderId)
        {
            if (Type != (int)NotificationType.OrderUpdate)
            {
                throw new DomainRuleViolationException(NotificationErrors.IncompatibleTypeCode, NotificationErrors.IncompatibleTypeMessage);
            }
            RelatedEntityID = orderId;
        }

        public void UpdateType(int newType)
        {
            if (newType <= 0)
            {
                throw new DomainValidationException(NotificationErrors.InvalidTypeCode, NotificationErrors.InvalidTypeMessage, nameof(newType));
            }

            Type = newType;
        }

        private static string Normalize(string value)
        {
            return value?.Trim() ?? string.Empty;
        }
    }
}
