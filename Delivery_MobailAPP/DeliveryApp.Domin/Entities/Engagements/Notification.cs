using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.EngagementEnams;
using DeliveryApp.Domain.DomainErrors.EngagementErrors;

namespace DeliveryApp.Domain.Entities.Engagements
{
    public class Notification
    {
        public NotificationID ID { get; private set; }
        public UserID UserID { get; private set; }

        public string Title { get; private set; } = string.Empty;
        public string Body { get; private set; } = string.Empty;

        public RelatedEntityType? RelatedEntityType { get; private set; }
        public Guid? RelatedEntityID { get; private set; }

        public bool IsRead { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ReadAt { get; private set; }

        private Notification() { }

        public Notification(UserID UserId, string title, string body, DateTimeOffset CreatedAtUtc,
            Guid? relatedEntityId = null, RelatedEntityType? relatedEntityType = null)
        {
            if (UserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(UserId));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAtUtc));

            if (relatedEntityType.HasValue && !Enum.IsDefined(typeof(RelatedEntityType), relatedEntityType.Value)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(relatedEntityType));

            ValidateRelatedEntity(relatedEntityId, relatedEntityType);

            ID = NotificationID.New();
            UserID = UserId;

            RelatedEntityID = relatedEntityId;
            RelatedEntityType = relatedEntityType;

            SetTitle(title);
            SetBody(body);

            CreatedAt = CreatedAtUtc;
            IsRead = false;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void MarkAsRead(DateTimeOffset readAtUtc)
        {
            if (IsRead) return;

            if (readAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(readAtUtc));

            if (readAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(readAtUtc));

            IsRead = true;
            ReadAt = readAtUtc;
        }

        // -------------------------
        //        Validation
        // -------------------------

        private static void ValidateRelatedEntity(Guid? relatedEntityId, RelatedEntityType? relatedEntityType)
        {
            if (relatedEntityId.HasValue && !relatedEntityType.HasValue) throw new DomainValidationException
                    (NotificationErrors.RelatedEntityRequiredCode, NotificationErrors.RelatedEntityRequiredMessage, nameof(relatedEntityType));

            if (!relatedEntityId.HasValue && relatedEntityType.HasValue) throw new DomainValidationException
                    (NotificationErrors.RelatedEntityRequiredCode, NotificationErrors.RelatedEntityRequiredMessage, nameof(relatedEntityId));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetTitle(string value)
        {
            value = NormalizeRequired(value, nameof(Title));

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Title));

            Title = value;
        }

        private void SetBody(string value)
        {
            value = NormalizeRequired(value, nameof(Body));

            if (value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Body));

            Body = value;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        private static string NormalizeRequired(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, fieldName);

            return value.Trim();
        }
    }
}