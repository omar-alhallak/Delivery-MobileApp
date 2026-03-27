using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.ModerationErrors;
using DeliveryApp.Domain.Enums.ModerationEnums.AccountWarningEnums;

namespace DeliveryApp.Domain.Entities.Moderation
{
    public class AccountWarning // يمثل تحذير إداري على جهة معينة
    {
        // -------------------------
        //            Key
        // -------------------------

        public AccountWarningID ID { get; private set; } // PK معرف التحذير

        // -------------------------
        //      Target Entity
        // -------------------------

        public WarningEntityType EntityType { get; private set; } // الجهة المستهدفة بالتحذير
        public Guid EntityID { get; private set; } // معرف الجهة

        // -------------------------
        //        Relation
        // -------------------------

        public OrderID? RelatedOrderID { get; private set; } // الطلب المرتبط بالتحذير

        // -------------------------
        //      Warning Details
        // -------------------------

        public WarningReason Reason { get; private set; } // سبب التحذير
        public string ReasonDetails { get; private set; } = null!; // تفاصيل إضافية عن سبب التحذير
        public WarningSeverity Severity { get; private set; } // شدة خطورة التحذير

        // -------------------------
        //         Decision
        // -------------------------

        public WarningDecision Decision { get; private set; } // قرار التحذير (معلق، مؤكد، مرفوض)د

        // -------------------------
        //          Status
        // -------------------------

        public bool IsActive { get; private set; } // هل التحذير ما يزال فعال

        // -------------------------
        //         Expiry
        // -------------------------

        public DateTimeOffset? ExpiresAt { get; private set; } // وقت انتهاء صلاحية التحذير

        // -------------------------
        //         Moderation
        // -------------------------

        public UserID CreatedByAdminID { get; private set; } // المشرف الذي أنشأ التحذير
        public UserID? DecidedByAdminID { get; private set; } // المشرف الذي حسم القرار

        // -------------------------
        //          Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء التحذير
        public DateTimeOffset? DecidedAt { get; private set; } // وقت اتخاذ القرار

        private AccountWarning() { }

        public AccountWarning(AccountWarningID id, WarningEntityType entityType, Guid entityId, WarningReason reason, string reasonDetails,
            WarningSeverity severity, UserID createdByAdminId, DateTimeOffset createdAtUtc, OrderID? relatedOrderId = null, DateTimeOffset? expiresAtUtc = null) // إنشاء تحذير جديد بحالة Pending
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (entityId == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(entityId));

            if (createdByAdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdByAdminId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (!Enum.IsDefined(typeof(WarningEntityType), entityType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(entityType));

            if (!Enum.IsDefined(typeof(WarningReason), reason)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reason));

            if (!Enum.IsDefined(typeof(WarningSeverity), severity)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(severity));

            if (expiresAtUtc.HasValue && expiresAtUtc.Value <= createdAtUtc) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(expiresAtUtc));

            ID = id;

            EntityType = entityType;
            EntityID = entityId;

            Reason = reason;
            SetReasonDetails(reasonDetails);
            Severity = severity;

            CreatedByAdminID = createdByAdminId;
            CreatedAt = createdAtUtc;

            RelatedOrderID = relatedOrderId;
            ExpiresAt = expiresAtUtc;

            Decision = WarningDecision.Pending;
            IsActive = true;
        }

        // ------------------------
        //        Decisions
        // ------------------------

        public void Confirm(UserID adminId, DateTimeOffset decidedAtUtc) // تأكيد التحذير
        {
            EnsurePending();

            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (decidedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(decidedAtUtc));

            if (decidedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(decidedAtUtc));

            Decision = WarningDecision.Confirmed;
            DecidedByAdminID = adminId;
            DecidedAt = decidedAtUtc;
        }

        public void Dismiss(UserID adminId, DateTimeOffset decidedAtUtc) // إيقاف التحذير
        {
            EnsurePending();

            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (decidedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(decidedAtUtc));

            if (decidedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(decidedAtUtc));

            Decision = WarningDecision.Dismissed;
            DecidedByAdminID = adminId;
            DecidedAt = decidedAtUtc;
            IsActive = false;
        }

        public void Deactivate() // تعطيل التحذير
        {
            if (!IsActive) throw new DomainRuleViolationException
                    (AccountWarningErrors.WarningAlreadyInactiveCode, AccountWarningErrors.WarningAlreadyInactiveMessage);

            IsActive = false;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        public bool IsExpired(DateTimeOffset now) // التحقق هل انتهت صلاحية التحذير
        {
            if (now == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(now));

            return ExpiresAt.HasValue && now >= ExpiresAt.Value;
        }

        private void EnsurePending() // التأكد أن التحذير ما زال بحالة Pending قبل حسمه
        {
            if (Decision != WarningDecision.Pending) throw new DomainRuleViolationException
                    (AccountWarningErrors.WarningAlreadyDecidedCode, AccountWarningErrors.WarningAlreadyDecidedMessage);
        }

        private void SetReasonDetails(string value) // ضبط تفاصيل سبب التحذير والتحقق من صحتها
        {
            value = NormalizeRequired(value, nameof(ReasonDetails));

            if (value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ReasonDetails));

            ReasonDetails = value;
        }

        private static string NormalizeRequired(string? value, string field) // تنظيف النص والتحقق من وجوده
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field);

            return value.Trim();
        }
    }
}