using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainErrors.AccountWarningErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.AccountWarningEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Moderation
{
    public class AccountWarning
    {
        public WarningID ID { get; private set; }

        public WarningEntityType EntityType { get; private set; }
        public Guid EntityID { get; private set; }

        public OrderID? RelatedOrderID { get; private set; }

        public WarningReason Reason { get; private set; }
        public WarningSeverity Severity { get; private set; }

        public WarningDecision Decision { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset? ExpiresAt { get; private set; }

        public UserID CreatedByAdminID { get; private set; }

        public UserID? DecidedByAdminID { get; private set; }

        public string? Notes { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? DecidedAt { get; private set; }

        private AccountWarning() { }

        public AccountWarning(
            WarningID id,
            WarningEntityType entityType,
            Guid entityId,
            WarningReason reason,
            WarningSeverity severity,
            UserID createdByAdminID,
            DateTimeOffset createdAtUtc,
            OrderID? relatedOrderID = null,
            DateTimeOffset? expiresAt = null,
            string? notes = null)
        {
            if (id.IsEmpty) throw new DomainValidationException
                (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (entityId == Guid.Empty) throw new DomainValidationException
                (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(entityId));

            if (createdByAdminID.IsEmpty) throw new DomainValidationException
                (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdByAdminID));

            if (createdAtUtc == default) throw new DomainValidationException
                (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (!Enum.IsDefined(typeof(WarningEntityType), entityType))
                throw new DomainValidationException
                (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(entityType));

            if (!Enum.IsDefined(typeof(WarningReason), reason))
                throw new DomainValidationException
                (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reason));

            if (!Enum.IsDefined(typeof(WarningSeverity), severity))
                throw new DomainValidationException
                (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(severity));

            ID = id;
            EntityType = entityType;
            EntityID = entityId;

            Reason = reason;
            Severity = severity;

            CreatedByAdminID = createdByAdminID;
            CreatedAt = createdAtUtc;

            RelatedOrderID = relatedOrderID;
            ExpiresAt = expiresAt;

            Notes = NormalizeOptional(notes);

            Decision = WarningDecision.Pending;
            IsActive = true;
        }

        // Decision
        // Only pending warnings can be confirmed or dismissed. Once a decision is made, it cannot be changed.
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
        // Decision
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
        }

        // Admin Actions
        // Only active warnings can be deactivated. Once deactivated, a warning cannot be reactivated.
        public void Deactivate()
        {
            if (!IsActive) throw new DomainRuleViolationException
                (AccountWarningErrors.WarningAlreadyInactiveCode,AccountWarningErrors.WarningAlreadyInactiveMessage);

            IsActive = false;
        }

        // Helpers

        public bool IsExpired(DateTimeOffset now)
            => ExpiresAt.HasValue && now >= ExpiresAt.Value;
        
        private void EnsurePending()
        {
            if (Decision != WarningDecision.Pending) throw new DomainRuleViolationException
                (AccountWarningErrors.WarningAlreadyDecidedCode,AccountWarningErrors.WarningAlreadyDecidedMessage);
        }

        private static string? NormalizeOptional(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
