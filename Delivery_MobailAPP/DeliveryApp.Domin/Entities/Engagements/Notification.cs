using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Domain.DomainErrors.EngagementErrors;

namespace DeliveryApp.Domain.Entities.Engagements
{
    public class Notification // يمثل الإشعار المرسل إلى المستخدم
    {
        // -------------------------
        //            Key
        // -------------------------

        public NotificationID ID { get; private set; } // PK معرف الإشعار
        public UserID UserID { get; private set; } // المستخدم صاحب الإشعار

        // -------------------------
        //         Content
        // -------------------------

        public string Title { get; private set; } = string.Empty; // عنوان الإشعار
        public string Body { get; private set; } = string.Empty; // نص الإشعار

        // -------------------------
        //      Related Entity
        // -------------------------

        public RelatedEntityType? RelatedEntityType { get; private set; } // نوع الكيان المرتبط فيه الإشعار
        public Guid? RelatedEntityID { get; private set; } // معرف الكيان المرتبط بالإشعار

        // -------------------------
        //          Status
        // -------------------------

        public bool IsRead { get; private set; } // هل تم قراءة الإشعار

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الإشعار
        public DateTimeOffset? ReadAt { get; private set; } // وقت قراءة الإشعار

        private Notification() { }

        public Notification(UserID userId, string title, string body, DateTimeOffset createdAtUtc, Guid? relatedEntityId = null, RelatedEntityType? relatedEntityType = null)
        {
            if (userId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (relatedEntityType.HasValue && !Enum.IsDefined(typeof(RelatedEntityType), relatedEntityType.Value)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(relatedEntityType));

            ValidateRelatedEntity(relatedEntityId, relatedEntityType);

            ID = NotificationID.New();
            UserID = userId;

            RelatedEntityID = relatedEntityId;
            RelatedEntityType = relatedEntityType;

            SetTitle(title);
            SetBody(body);

            CreatedAt = createdAtUtc;
            IsRead = false;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void MarkAsRead(DateTimeOffset readAtUtc) // تمت قراءة الإشعار 
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

        private static void ValidateRelatedEntity(Guid? relatedEntityId, RelatedEntityType? relatedEntityType) // التحقق من صحة الربط بين الإشعار والكيان
        {
            if (relatedEntityId.HasValue && relatedEntityId.Value == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(relatedEntityId));

            if (relatedEntityId.HasValue && !relatedEntityType.HasValue) throw new DomainValidationException
                    (NotificationErrors.RelatedEntityRequiredCode, NotificationErrors.RelatedEntityRequiredMessage, nameof(relatedEntityType));

            if (!relatedEntityId.HasValue && relatedEntityType.HasValue) throw new DomainValidationException
                    (NotificationErrors.RelatedEntityRequiredCode, NotificationErrors.RelatedEntityRequiredMessage, nameof(relatedEntityId));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetTitle(string value) // إدخال عنوان الإشعار
        {
            value = NormalizeRequired(value, nameof(Title));

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Title));

            Title = value;
        }

        private void SetBody(string value) // إدخال نص الإشعار
        {
            value = NormalizeRequired(value, nameof(Body));

            if (value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Body));

            Body = value;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        private static string NormalizeRequired(string? value, string fieldName) // تنظيف النص
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, fieldName);

            return value.Trim();
        }
    }
}