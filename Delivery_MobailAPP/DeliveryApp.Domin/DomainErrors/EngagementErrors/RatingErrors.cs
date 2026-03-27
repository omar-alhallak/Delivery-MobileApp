namespace DeliveryApp.Domain.DomainErrors.EngagementErrors
{
    public static class RatingErrors
    {
        // انتهى مدة تعديل على التقييم
        public const string EditExpiredCode = "Rating.Edit_Expired";
        public const string EditExpiredMessage = "Ratings cant be edited after 24 hours.";

        // لا يمكنك تقييم نفسك
        public const string SelfRatingCode = "Rating.Self_Rating";
        public const string SelfRatingMessage = "A user cant rate himself.";
    }
}