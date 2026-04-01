namespace DeliveryApp.Domain.DomainErrors.ModerationErrors
{
    public static class ComplaintErrors
    {
        // لا يمكنك مراجعة الشكوى إلى إذا كانت معلقة
        public const string ComplaintMustBePendingCode = "Complaint_Must_BE_Pending";
        public const string ComplaintMustBePendingMessage = "Only pending complaints can be modified or reviewed.";
    }
}