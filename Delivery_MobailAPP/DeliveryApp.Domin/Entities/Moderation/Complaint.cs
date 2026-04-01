using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.ModerationErrors;
using DeliveryApp.Domain.Enums.ModerationEnums.ComplaintEnums;

namespace DeliveryApp.Domain.Entities.Moderation
{
    public class Complaint // يمثل شكوى مقدمة من مستخدم ضد جهة معينة
    {
        // -------------------------
        //            Key
        // -------------------------

        public ComplaintID ID { get; private set; } // PK معرف الشكوى

        // -------------------------
        //         Creator
        // -------------------------

        public UserID CreatedByUserID { get; private set; } // المستخدم الذي أنشأ الشكوى

        // -------------------------
        //         Target
        // -------------------------

        public ComplaintTargetType TargetType { get; private set; } // نوع الجهة المستهدفة بالشكوى
        public Guid TargetID { get; private set; } // معرف الجهة المستهدفة

        // -------------------------
        //         Relation
        // -------------------------

        public OrderID OrderID { get; private set; } // الطلب المرتبط بالشكوى

        // -------------------------
        //      Complaint Details
        // -------------------------

        public ComplaintReason Reason { get; private set; } // سبب الشكوى
        public string Message { get; private set; } = null!; // وصف الشكوى من المستخدم

        // -------------------------
        //          Review
        // -------------------------

        public ComplaintStatus Status { get; private set; } // حالة الشكوى

        public UserID? ReviewedByAdminID { get; private set; } // المشرف الذي راجع الشكوى
        public string? AdminResponse { get; private set; } // رد المشرف على الشكوى

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الشكوى
        public DateTimeOffset? ResolvedAt { get; private set; } // وقت حسم الشكوى

        private Complaint() { }

        public Complaint(ComplaintID id, UserID createdByUserId, ComplaintTargetType targetType, Guid targetId, OrderID orderId,
            ComplaintReason reason, string message, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (createdByUserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdByUserId));

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

            ID = id;
            CreatedByUserID = createdByUserId;
            TargetType = targetType;
            TargetID = targetId;
            OrderID = orderId;
            Reason = reason;
            CreatedAt = createdAtUtc;

            SetMessage(message);

            Status = ComplaintStatus.Pending;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void UpdateReason(ComplaintReason reason) // تعديل سبب الشكوى
        {
            EnsurePending();

            if (!Enum.IsDefined(typeof(ComplaintReason), reason)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reason));

            Reason = reason;
        }

        public void UpdateMessage(string message) // تعديل وصف الشكوى
        {
            EnsurePending();
            SetMessage(message);
        }

        public void Resolve(UserID reviewedByAdminId, string adminResponse, DateTimeOffset resolvedAtUtc) // اعتماد الشكوى
        {
            EnsurePending();

            if (reviewedByAdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(reviewedByAdminId));

            if (resolvedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(resolvedAtUtc));

            if (resolvedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(resolvedAtUtc));

            SetAdminResponse(adminResponse);

            Status = ComplaintStatus.Resolved;
            ReviewedByAdminID = reviewedByAdminId;
            ResolvedAt = resolvedAtUtc;
        }

        public void Reject(UserID reviewedByAdminId, string adminResponse, DateTimeOffset resolvedAtUtc) // رفض الشكوى
        {
            EnsurePending();

            if (reviewedByAdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(reviewedByAdminId));

            if (resolvedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(resolvedAtUtc));

            if (resolvedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(resolvedAtUtc));

            SetAdminResponse(adminResponse);

            Status = ComplaintStatus.Rejected;
            ReviewedByAdminID = reviewedByAdminId;
            ResolvedAt = resolvedAtUtc;
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetMessage(string value) // إدخال نص الشكوى
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(Message));

            value = value.Trim();

            if (value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Message));

            Message = value;
        }

        private void SetAdminResponse(string value) // إدخال رد المشرفين على شكوى
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(AdminResponse));

            value = value.Trim();

            if (value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(AdminResponse));

            AdminResponse = value;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        private void EnsurePending() // التأكد أن الشكوى لم تحسم بعد
        {
            if (Status != ComplaintStatus.Pending) throw new DomainRuleViolationException
                    (ComplaintErrors.ComplaintMustBePendingCode, ComplaintErrors.ComplaintMustBePendingMessage);
        }
    }
}