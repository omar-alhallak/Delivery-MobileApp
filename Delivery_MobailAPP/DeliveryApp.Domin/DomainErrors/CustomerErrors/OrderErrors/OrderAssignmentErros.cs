namespace DeliveryApp.Domain.DomainErrors.OrderErrors
{
    public static class OrderAssignmentErrors
    {
        // إذا حذف الطلب لا يمكن تعديله
        public const string CantModifyRemovedAssignmentCode = "OrderAssignment.Cant_Modify_Removed_Assignment";
        public const string CantModifyRemovedAssignmentMessage = "Removed assignment cant be modified.";

        // الطلب تم قبوله مسبقاً
        public const string AssignmentAlreadyAcceptedCode = "OrderAssignment.Already_Accepted";
        public const string AssignmentAlreadyAcceptedMessage = "Assignment is already accepted.";

        // الطلب تم رفضه مسبقاً
        public const string AssignmentAlreadyRejectedCode = "OrderAssignment.Already_Rejected";
        public const string AssignmentAlreadyRejectedMessage = "Assignment is already rejected.";

        // الطلب تم حذفه مسبقاً
        public const string AssignmentAlreadyRemovedCode = "OrderAssignment.Already_Removed";
        public const string AssignmentAlreadyRemovedMessage = "Assignment is already removed.";

        // الطلب تم رفضه لا يمكنك قبوله
        public const string CantAcceptRejectedAssignmentCode = "OrderAssignment.Cant_Accept_Rejected_Assignment";
        public const string CantAcceptRejectedAssignmentMessage = "Rejected assignment cant be accepted.";

        // الطلب تم قبوله لا يمكنك رفضه
        public const string CantRejectAcceptedAssignmentCode = "OrderAssignment.Cant_Reject_Accepted_Assignment";
        public const string CantRejectAcceptedAssignmentMessage = "Accepted assignment cant be rejected.";
    }
}