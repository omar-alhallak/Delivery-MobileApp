using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.DomainErrors.EngagementsErrors
{
    internal class RatingErrors
    {
        public const string EditExpiredCode = "Rating.Edit_Expired";
        public const string EditExpiredMessage = "Ratings cannot be edited after 24 hours.";

        // قاعدة التقييم الذاتي
        public const string SelfRatingCode = "Rating.Self_Rating";
        public const string SelfRatingMessage = "It is illogical for a user to evaluate himself.";

        // قاعدة عدد النجوم
        public const string InvalidStarsCode = "Rating.Invalid_Stars";
        public const string InvalidStarsMessage = "The rating should be between 1 and 5 stars.";

        // قاعدة طول التعليق
        public const string CommentTooLongCode = "Rating.Comment_Too_Long";
        public const string CommentTooLongMessage = "The comment exceeds the maximum allowed length.";

        public const string RelatedEntityRequiredCode = "Notification.Related_Entity_Required";
        public const string RelatedEntityRequiredMessage = "RelatedEntityID is required when type is specified.";

        public const string InvalidTypeCode = "Notification.Invalid_Type";
        public const string InvalidTypeMessage = "The specified type is not a valid member of the enumeration.";

        public const string IncompleteCode = "Rating.Incomplete";
        public const string IncompleteMessage = "This rating is incomplete and cannot be submitted.";
    }
}

