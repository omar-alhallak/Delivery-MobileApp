using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.DomainErrors.EngagementsErrors
{
    internal class RatingErrors
    {
        public const string EditExpiredCode = "Rating.EditExpired";
        public const string EditExpiredMessage = "Ratings cannot be edited after 24 hours.";

        // التقييم الذاتي (منطق جبار)
        public const string SelfRatingCode = "Rating.SelfRating";
        public const string SelfRatingMessage = "It is illogical for a user to evaluate himself.";

        // قاعدة النجوم (الـ Enum)
        public const string InvalidStarsCode = "Rating.InvalidStars";
        public const string InvalidStarsMessage = "The rating must be a valid value between Bad (1) and Excellent (5).";

        // طول التعليق
        public const string CommentTooLongCode = "Rating.CommentTooLong";
        public const string CommentTooLongMessage = "The comment exceeds the maximum allowed length (500 characters).";

        public const string InvalidTypeCode = "Rating.InvalidType";
        public const string InvalidTypeMessage = "The specified target type is not a valid rating category.";
    }
}

