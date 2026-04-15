using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.OrderEnums;
using DeliveryApp.Domain.DomainErrors.OrderErrors;

namespace DeliveryApp.Domain.Entities.Customers.Orders
{
    public class OrderAssignment // يمثل إسناد طلب إلى سائق
    {
        // -------------------------
        //            Key
        // -------------------------

        public Guid ID { get; private set; } // pk معرف الإسناد

        // -------------------------
        //         Relations
        // -------------------------

        public OrderID OrderID { get; private set; } // الطلب المرتبط بهذا الإسناد
        public UserID DriverID { get; private set; } // السائق الذي أسند إليه الطلب

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset AssignedAt { get; private set; } // وقت إنشاء الإسناد

        // -------------------------
        //          Status
        // -------------------------

        public OrderAssignmentStatus Status { get; private set; } // حالة الإسناد الحالية

        // -------------------------
        //          Remove
        // -------------------------

        public DateTimeOffset? RemovedAt { get; private set; } // وقت إزالة الإسناد
        public string? RemoveReason { get; private set; } // سبب إزالة الإسناد

        public bool IsRemoved => Status == OrderAssignmentStatus.Removed; // هل الإسناد تمت إزالته

        private OrderAssignment() { }

        public OrderAssignment(Guid id, OrderID orderId, UserID driverId, DateTimeOffset assignedAtUtc)
        {
            if (id == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (orderId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderId));

            if (driverId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(driverId));

            if (assignedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(assignedAtUtc));

            ID = id;
            OrderID = orderId;
            DriverID = driverId;
            AssignedAt = assignedAtUtc;

            Status = OrderAssignmentStatus.Pending;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Accept() // قبول السائق
        {
            EnsureNotRemoved();

            if (Status == OrderAssignmentStatus.Accepted) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.AssignmentAlreadyAcceptedCode, OrderAssignmentErrors.AssignmentAlreadyAcceptedMessage);

            if (Status == OrderAssignmentStatus.Rejected) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.CantAcceptRejectedAssignmentCode, OrderAssignmentErrors.CantAcceptRejectedAssignmentMessage);

            Status = OrderAssignmentStatus.Accepted;
        }

        public void Reject() // رفض السائق
        {
            EnsureNotRemoved();

            if (Status == OrderAssignmentStatus.Rejected) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.AssignmentAlreadyRejectedCode, OrderAssignmentErrors.AssignmentAlreadyRejectedMessage);

            if (Status == OrderAssignmentStatus.Accepted) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.CantRejectAcceptedAssignmentCode, OrderAssignmentErrors.CantRejectAcceptedAssignmentMessage);

            Status = OrderAssignmentStatus.Rejected;
        }

        public void Remove(string reason, DateTimeOffset removedAtUtc) // إزالة الإسناد من الطلب
        {
            if (IsRemoved) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.AssignmentAlreadyRemovedCode, OrderAssignmentErrors.AssignmentAlreadyRemovedMessage);

            if (removedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(removedAtUtc));

            if (removedAtUtc < AssignedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(removedAtUtc));

            SetRemoveReason(reason);

            RemovedAt = removedAtUtc;
            Status = OrderAssignmentStatus.Removed;
        }

        // -------------------------
        //        Validation
        // -------------------------

        private void EnsureNotRemoved() // التأكد أن الإسناد لم تتم إزالته
        {
            if (IsRemoved) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.CantModifyRemovedAssignmentCode, OrderAssignmentErrors.CantModifyRemovedAssignmentMessage);
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetRemoveReason(string value) // إدخال سبب الإزالة
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(RemoveReason));

            value = value.Trim();

            if (value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(RemoveReason));

            RemoveReason = value;
        }
    }
}