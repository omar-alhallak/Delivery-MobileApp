namespace DeliveryApp.Domain.DomainErrors.EngagementErrors
{
    public static class NotificationErrors
    {
        // جهة المقيمة مطلوبة
        public const string RelatedEntityRequiredCode = "NOTIFICATION.RELATED_ENTITY_REQUIRED";
        public const string RelatedEntityRequiredMessage = "Related entity type and related entity ID must be provided together.";
    }
}