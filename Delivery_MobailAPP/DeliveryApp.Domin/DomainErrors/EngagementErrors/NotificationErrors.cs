namespace DeliveryApp.Domain.DomainErrors.EngagementErrors
{
    public static class NotificationErrors
    {
        // جهة المقيمة مطلوبة
        public const string RelatedEntityRequiredCode = "Notification.Related_Entity_Required";
        public const string RelatedEntityRequiredMessage = "Related entity type and related entity ID must be provided together.";
    }
}