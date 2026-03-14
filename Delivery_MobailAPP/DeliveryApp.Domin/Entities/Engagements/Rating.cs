using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainErrors.EngagementsErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.EngagementsEnams;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Domain.Entities.Feedback
{
    public class Rating
    {
        public StrongID<RatingTag> Id { get; private set; }
        public StrongID<OrderTag> OrderId { get; private set; }
        public StrongID<UserTag> RaterUserId { get; private set; }

        public RatedEntityType TargetType { get; private set; }

        public StrongID<UserTag> RatedEntityId { get; private set; }

        public RatingStars Stars { get; private set; }        
        public string? Comment { get; private set; }   

        public DateTimeOffset CreatedAt { get; private set; }

        private Rating() { }

   
        public Rating(StrongID<OrderTag> orderId, StrongID<UserTag> raterId, StrongID<UserTag> targetId, RatingStars stars, RatedEntityType targetType, string? comment = null )
        {

            if (!Enum.IsDefined(typeof(RatedEntityType), targetType))
                throw new DomainValidationException(
                    RatingErrors.InvalidTypeCode, // تأكد إن هاد الكود موجود بملف الإيرور
                    RatingErrors.InvalidTypeMessage,
                    nameof(TargetType));
            if (orderId.IsEmpty)
                throw new DomainValidationException(ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderId));

            if (raterId.IsEmpty)
                throw new DomainValidationException(ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(raterId));

            if (targetId.IsEmpty)
                throw new DomainValidationException(ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(targetId));

            if (raterId == targetId)
                throw new DomainRuleViolationException(RatingErrors.SelfRatingCode, RatingErrors.SelfRatingMessage);

            Id = StrongID<RatingTag>.New();
            OrderId = orderId;
            RaterUserId = raterId;
            RatedEntityId = targetId;
            TargetType = targetType;
            SetStars(stars);
            SetComment(comment);

            CreatedAt = DateTimeOffset.UtcNow;
        }
        public void Update(RatingStars newStars, string? newComment)
        {
            if (DateTimeOffset.UtcNow > CreatedAt.AddDays(1))
                throw new DomainRuleViolationException(RatingErrors.EditExpiredCode, RatingErrors.EditExpiredMessage);

            SetStars(newStars);
            SetComment(newComment);
        }
        // 5. ميثودز الحماية والتعديل (Behavior)
        private void SetStars(RatingStars stars)
        {
            if (!Enum.IsDefined(typeof(RatingStars), stars))
                throw new DomainValidationException(RatingErrors.InvalidStarsCode, RatingErrors.InvalidStarsMessage, nameof(Stars));

            Stars = stars;
        }

        private void SetComment(string? comment)
        {
            string? normalized = Normalize(comment);

            if (normalized != null && normalized.Length > 500)
                throw new DomainValidationException(RatingErrors.CommentTooLongCode, RatingErrors.CommentTooLongMessage, nameof(Comment));

            Comment = normalized;
        }
        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}