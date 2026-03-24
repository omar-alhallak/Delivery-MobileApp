using System;
using DeliveryApp.Domain.Enums.Order;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.OrderErrors;

namespace DeliveryApp.Domain.Entities.Orders
{
    public class OrderAssignment
    {
        public Guid ID { get; private set; }

        public OrderID OrderID { get; private set; }
        public UserID DriverID { get; private set; }

        public DateTimeOffset AssignedAt { get; private set; }

        public OrderAssignmentStatus Status { get; private set; }

        public DateTimeOffset? RemovedAt { get; private set; }
        public string? RemoveReason { get; private set; }

        public bool IsRemoved => Status == OrderAssignmentStatus.Removed;

        private OrderAssignment() { }

        public OrderAssignment(Guid id, OrderID OrderId, UserID DriverId, DateTimeOffset AssignedAtUtc)
        {
            if (id == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (OrderId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(OrderId));

            if (DriverId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(DriverId));

            if (AssignedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(AssignedAtUtc));

            ID = id;
            OrderID = OrderId;
            DriverID = DriverId;
            AssignedAt = AssignedAtUtc;

            Status = OrderAssignmentStatus.Pending;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Accept()
        {
            CheckNotRemoved();

            if (Status == OrderAssignmentStatus.Accepted) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.AssignmentAlreadyAcceptedCode, OrderAssignmentErrors.AssignmentAlreadyAcceptedMessage);

            if (Status == OrderAssignmentStatus.Rejected) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.CantAcceptRejectedAssignmentCode, OrderAssignmentErrors.CantAcceptRejectedAssignmentMessage);

            Status = OrderAssignmentStatus.Accepted;
        }

        public void Reject()
        {
            CheckNotRemoved();

            if (Status == OrderAssignmentStatus.Rejected) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.AssignmentAlreadyRejectedCode, OrderAssignmentErrors.AssignmentAlreadyRejectedMessage);

            if (Status == OrderAssignmentStatus.Accepted) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.CantRejectAcceptedAssignmentCode, OrderAssignmentErrors.CantRejectAcceptedAssignmentMessage);

            Status = OrderAssignmentStatus.Rejected;
        }

        public void Remove(string reason, DateTimeOffset RemovedAtUtc)
        {
            if (IsRemoved) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.AssignmentAlreadyRemovedCode, OrderAssignmentErrors.AssignmentAlreadyRemovedMessage);

            if (RemovedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(RemovedAtUtc));

            SetRemoveReason(reason);

            RemovedAt = RemovedAtUtc;
            Status = OrderAssignmentStatus.Removed;
        }

        // -------------------------
        //        Validation
        // -------------------------

        private void CheckNotRemoved()
        {
            if (IsRemoved) throw new DomainRuleViolationException
                    (OrderAssignmentErrors.CantModifyRemovedAssignmentCode, OrderAssignmentErrors.CantModifyRemovedAssignmentMessage);
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetRemoveReason(string value)
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