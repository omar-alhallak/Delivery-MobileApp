using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.ModerationErrors;
using DeliveryApp.Domain.Enums.ModerationEnums.AccountWarningEnums;

namespace DeliveryApp.Domain.Entities.Moderation
{
    public class AccountWarning
    {
        public WarningID ID { get; private set; }

        public WarningEntityType EntityType { get; private set; }
        public Guid EntityID { get; private set; }

        public OrderID? RelatedOrderID { get; private set; }

        public WarningReason Reason { get; private set; }
        public string ReasonDetails { get; private set; } = null!;
        public WarningSeverity Severity { get; private set; }

        public WarningDecision Decision { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset? ExpiresAt { get; private set; }

        public UserID CreatedByAdminID { get; private set; }
        public UserID? DecidedByAdminID { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? DecidedAt { get; private set; }

        private AccountWarning() { }

        public AccountWarning(WarningID id, WarningEntityType entityType, Guid EntityId, WarningReason reason,
            string reasonDetails, WarningSeverity severity, UserID CreatedByAdminid, DateTimeOffset CreatedAtUtc,
            OrderID? RelatedOrderid = null, DateTimeOffset? ExpiresAtUtc = null)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (EntityId == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(EntityId));

            if (CreatedByAdminid.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedByAdminid));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAtUtc));

            if (!Enum.IsDefined(typeof(WarningEntityType), entityType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(entityType));

            if (!Enum.IsDefined(typeof(WarningReason), reason)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reason));

            if (!Enum.IsDefined(typeof(WarningSeverity), severity)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(severity));

            if (ExpiresAtUtc.HasValue && ExpiresAtUtc.Value <= CreatedAtUtc) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(ExpiresAtUtc));

            ID = id;

            EntityType = entityType;
            EntityID = EntityId;

            Reason = reason;
            SetReasonDetails(reasonDetails);
            Severity = severity;

            CreatedByAdminID = CreatedByAdminid;
            CreatedAt = CreatedAtUtc;

            RelatedOrderID = RelatedOrderid;
            ExpiresAt = ExpiresAtUtc;

            Decision = WarningDecision.Pending;
            IsActive = true;
        }

        // ------------------------
        //        Decisions
        // ------------------------

        public void Confirm(UserID adminId, DateTimeOffset decidedAtUtc)
        {
            EnsurePending();

            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (decidedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(decidedAtUtc));

            Decision = WarningDecision.Confirmed;
            DecidedByAdminID = adminId;
            DecidedAt = decidedAtUtc;
        }

        public void Dismiss(UserID adminId, DateTimeOffset decidedAtUtc)
        {
            EnsurePending();

            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (decidedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(decidedAtUtc));

            Decision = WarningDecision.Dismissed;
            DecidedByAdminID = adminId;
            DecidedAt = decidedAtUtc;
            IsActive = false;
        }

        public void Deactivate()
        {
            if (!IsActive) throw new DomainRuleViolationException
                    (AccountWarningErrors.WarningAlreadyInactiveCode, AccountWarningErrors.WarningAlreadyInactiveMessage);

            IsActive = false;
        }

        // -------------------------
        //        Helpers
        // -------------------------

        public bool IsExpired(DateTimeOffset now) => ExpiresAt.HasValue && now >= ExpiresAt.Value;

        private void EnsurePending()
        {
            if (Decision != WarningDecision.Pending) throw new DomainRuleViolationException
                    (AccountWarningErrors.WarningAlreadyDecidedCode, AccountWarningErrors.WarningAlreadyDecidedMessage);
        }

        private void SetReasonDetails(string value)
        {
            value = NormalizeRequired(value, nameof(ReasonDetails));

            if (value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ReasonDetails));

            ReasonDetails = value;
        }

        private static string NormalizeRequired(string? value, string field)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field);

            return value.Trim();
        }
    }
}