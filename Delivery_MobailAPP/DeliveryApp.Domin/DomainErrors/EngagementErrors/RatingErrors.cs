using System;

namespace DeliveryApp.Domain.DomainErrors.EngagementErrors
{
    public static class RatingErrors
    {
        public const string EditExpiredCode = "RATING.EDIT_EXPIRED";
        public const string EditExpiredMessage = "Ratings cannot be edited after 24 hours.";

        public const string SelfRatingCode = "RATING.SELF_RATING";
        public const string SelfRatingMessage = "A user cannot rate himself.";
    }
}