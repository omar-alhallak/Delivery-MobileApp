using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.ModerationErrors;
using DeliveryApp.Domain.Enums.ModerationEnums.ComplaintEnums;

namespace DeliveryApp.Domain.Entities.Moderation
{
    public class Complaint
    {
        public ComplaintID ID { get; private set; }

        public UserID CreatedByUserID { get; private set; }

        public ComplaintTargetType TargetType { get; private set; }
        public Guid TargetID { get; private set; }

        public OrderID OrderID { get; private set; }

        public ComplaintReason Reason { get; private set; }
        public string Message { get; private set; } = null!;

        public ComplaintStatus Status { get; private set; }

        public UserID? ReviewedByAdminID { get; private set; }
        public string? AdminResponse { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ResolvedAt { get; private set; }

        private Complaint() { }

        public Complaint(ComplaintID id, UserID CreatedByUserid, ComplaintTargetType targetType, Guid TargetId,
            OrderID OrderId, ComplaintReason reason, string message, DateTimeOffset CreatedAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (CreatedByUserid.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedByUserid));

            if (!Enum.IsDefined(typeof(ComplaintTargetType), targetType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(targetType));

            if (TargetId == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(TargetId));

            if (OrderId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(OrderId));

            if (!Enum.IsDefined(typeof(ComplaintReason), reason)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reason));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAtUtc));

            ID = id;
            CreatedByUserID = CreatedByUserid;
            TargetType = targetType;
            TargetID = TargetId;
            OrderID = OrderId;
            Reason = reason;
            CreatedAt = CreatedAtUtc;

            SetMessage(message);

            Status = ComplaintStatus.Pending;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void UpdateReason(ComplaintReason reason)
        {
            EnsurePending();

            if (!Enum.IsDefined(typeof(ComplaintReason), reason)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reason));

            Reason = reason;
        }

        public void UpdateMessage(string message)
        {
            EnsurePending();
            SetMessage(message);
        }

        public void Resolve(UserID reviewedByAdminID, string adminResponse, DateTimeOffset resolvedAtUtc)
        {
            EnsurePending();

            if (reviewedByAdminID.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(reviewedByAdminID));

            if (resolvedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(resolvedAtUtc));

            if (resolvedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(resolvedAtUtc));

            SetAdminResponse(adminResponse);

            Status = ComplaintStatus.Resolved;
            ReviewedByAdminID = reviewedByAdminID;
            ResolvedAt = resolvedAtUtc;
        }

        public void Reject(UserID ReviewedByAdminid, string adminResponse, DateTimeOffset resolvedAtUtc)
        {
            EnsurePending();

            if (ReviewedByAdminid.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ReviewedByAdminid));

            if (resolvedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(resolvedAtUtc));

            if (resolvedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(resolvedAtUtc));

            SetAdminResponse(adminResponse);

            Status = ComplaintStatus.Rejected;
            ReviewedByAdminID = ReviewedByAdminid;
            ResolvedAt = resolvedAtUtc;
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetMessage(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(Message));

            value = value.Trim();

            if (value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Message));

            Message = value;
        }

        private void SetAdminResponse(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(AdminResponse));

            value = value.Trim();

            if (value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(AdminResponse));

            AdminResponse = value;
        }

        private void EnsurePending()
        {
            if (Status != ComplaintStatus.Pending) throw new DomainRuleViolationException
                    (ComplaintErrors.ComplaintMustBePendingCode, ComplaintErrors.ComplaintMustBePendingMessage);
        }
    }
}