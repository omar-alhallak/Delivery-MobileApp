using DeliveryApp.Domain.DomainErrors.EngagementsErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.Customers.Order;
using DeliveryApp.Domain.Entities.DriverRequest;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.EngagementsEnams;

namespace DeliveryApp.Domain.Entities.Feedback
{
    public class Rating
    {
        public Guid RatingId { get; private set; }

        public Guid OrderId { get; private set; }
        public Order? Order { get; private set; }

        public Guid RaterUserId { get; private set; }

        public User? RaterUser { get; private set; }
        public RatedEntityType RatedEntityType { get; private set; }

        public Guid RatedEntityId { get; private set; }
        public User? RaterEntity { get; private set; }

        public RatingStars Stars { get; private set; }

        public string? Comment { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Rating() { }

        public void Create(Guid orderId, Guid raterId, Guid ratedId, RatedEntityType ratedType, int stars, string? comment)
        {

            if (raterId == ratedId)
            {
                throw new DomainRuleViolationException(RatingErrors.SelfRatingCode,RatingErrors.SelfRatingMessage);
            }
            RatingId = Guid.NewGuid();
            OrderId = orderId;
            RaterUserId = raterId;
            RatedEntityId = ratedId;
            RatedEntityType = ratedType;
            CreatedAt = DateTimeOffset.UtcNow;
            var normalizedComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        }

        public void Update(int newStars, string? newComment)
        {
            if (DateTimeOffset.UtcNow > CreatedAt.AddHours(24))
            {
                throw new DomainRuleViolationException( RatingErrors.EditExpiredCode,RatingErrors.EditExpiredMessage);
            }

            // 2. فحص طول التعليق (Validation)
            if (newComment != null && newComment.Length > RatingConstraints.MaxCommentLength)
            {
                throw new DomainValidationException( RatingErrors.CommentTooLongCode,RatingErrors.CommentTooLongMessage,
                    nameof(newComment));
            }

            // 3. فحص النجوم (Validation)
            if (newStars < RatingConstraints.MinStars || newStars > RatingConstraints.MaxStars)
            {
                throw new DomainValidationException( RatingErrors.InvalidStarsCode, RatingErrors.InvalidStarsMessage,
                    nameof(newStars));
            }

            Stars = (RatingStars)newStars;
            Comment = Normalize(newComment);
        }
        public void UpdateRelatedEntity(RatedEntityType? newType, Guid? newId)
        {
            if (newType.HasValue && !newId.HasValue)
            {
                throw new DomainRuleViolationException(RatingErrors.RelatedEntityRequiredCode,RatingErrors.RelatedEntityRequiredMessage);
            }

            if (newType.HasValue && !Enum.IsDefined(typeof(RatedEntityType), newType.Value))
            {
                throw new DomainValidationException(RatingErrors.InvalidTypeCode, RatingErrors.InvalidTypeMessage, nameof(newType));
            }

            if (newType.HasValue) RatedEntityType = newType.Value;
            if (newId.HasValue) RatedEntityId = newId.Value;
        }
        public bool IsReadyToSubmit => RatedEntityId != Guid.Empty && (int)Stars >= 1;

        // ميثود الفحص قبل الحفظ
        public void ValidateForSubmission()
        {
            if (!IsReadyToSubmit)
                throw new DomainRuleViolationException(RatingErrors.IncompleteCode, RatingErrors.IncompleteMessage);
        }
        public static class RatingConstraints
        {
            public const int MinStars = 1;
            public const int MaxStars = 5;
            public const int MaxCommentLength = 1000;
        }
        private string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    } 
}