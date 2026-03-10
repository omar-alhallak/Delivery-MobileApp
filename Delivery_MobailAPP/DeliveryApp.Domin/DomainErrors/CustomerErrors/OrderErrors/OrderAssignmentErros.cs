using System;

namespace DeliveryApp.Domain.DomainErrors.OrderErrors
{
    public static class OrderAssignmentErrors
    {
        public const string CantModifyRemovedAssignmentCode = "OrderAssignment.Cant_Modify_Removed_Assignment";
        public const string CantModifyRemovedAssignmentMessage = "Removed assignment cant be modified.";

        public const string AssignmentAlreadyAcceptedCode = "OrderAssignment.Already_Accepted";
        public const string AssignmentAlreadyAcceptedMessage = "Assignment is already accepted.";

        public const string AssignmentAlreadyRejectedCode = "OrderAssignment.Already_Rejected";
        public const string AssignmentAlreadyRejectedMessage = "Assignment is already rejected.";

        public const string AssignmentAlreadyRemovedCode = "OrderAssignment.Already_Removed";
        public const string AssignmentAlreadyRemovedMessage = "Assignment is already removed.";

        public const string CantAcceptRejectedAssignmentCode = "OrderAssignment.Cant_Accept_Rejected_Assignment";
        public const string CantAcceptRejectedAssignmentMessage = "Rejected assignment cant be accepted.";

        public const string CantRejectAcceptedAssignmentCode = "OrderAssignment.Cant_Reject_Accepted_Assignment";
        public const string CantRejectAcceptedAssignmentMessage = "Accepted assignment cant be rejected.";
    }
}