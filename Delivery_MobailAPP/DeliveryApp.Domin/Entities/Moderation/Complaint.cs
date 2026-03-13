using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainErrors.ComplaintErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.ComplaintEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Complaint(
            ComplaintID id,
            UserID createdByUserID,
            ComplaintTargetType targetType,
            Guid targetId,
            OrderID orderId,
            ComplaintReason reason,
            string message,
            DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (createdByUserID.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdByUserID));

            if (!Enum.IsDefined(typeof(ComplaintTargetType), targetType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(targetType));

            if (targetId == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(targetId));

            if (orderId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderId));

            if (!Enum.IsDefined(typeof(ComplaintReason), reason)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reason));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (targetType == ComplaintTargetType.User && targetId == createdByUserID.Value) throw new DomainRuleViolationException
                    (ComplaintErrors.ComplaintCannotTargetSelfCode, ComplaintErrors.ComplaintCannotTargetSelfMessage);

            ID = id;
            CreatedByUserID = createdByUserID;
            TargetType = targetType;
            TargetID = targetId;
            OrderID = orderId;
            Reason = reason;
            CreatedAt = createdAtUtc;

            SetMessage(message);

            Status = ComplaintStatus.Pending;
        }

        //         Behavior
        //         Only pending complaints can be updated or reviewed
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

            SetAdminResponse(adminResponse);

            Status = ComplaintStatus.Resolved;
            ReviewedByAdminID = reviewedByAdminID;
            ResolvedAt = resolvedAtUtc;
        }

        public void Reject(UserID reviewedByAdminID, string adminResponse, DateTimeOffset resolvedAtUtc)
        {
            EnsurePending();

            if (reviewedByAdminID.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(reviewedByAdminID));

            if (resolvedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(resolvedAtUtc));

            SetAdminResponse(adminResponse);

            Status = ComplaintStatus.Rejected;
            ReviewedByAdminID = reviewedByAdminID;
            ResolvedAt = resolvedAtUtc;
        }

        //         Validation

        private void EnsurePending()
        {
            if (Status != ComplaintStatus.Pending) throw new DomainRuleViolationException
                    (ComplaintErrors.ComplaintMustBePendingCode, ComplaintErrors.ComplaintMustBePendingMessage);
        }

        //         Setters

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
            if (string.IsNullOrWhiteSpace(value)) throw new DomainRuleViolationException
                    (ComplaintErrors.AdminResponseRequiredCode, ComplaintErrors.AdminResponseRequiredMessage);

            value = value.Trim();

            if (value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(AdminResponse));

            AdminResponse = value;

        }
    }
}