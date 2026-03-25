using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainErrors.EngagementErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.EngagementEnams;
using System;

namespace DeliveryApp.Domain.Entities.Engagements
{
    public class Rating
    {
        public RatingID ID { get; private set; }
        public OrderID OrderID { get; private set; }
        public UserID RaterUserID { get; private set; }

        public RatedEntityType TargetType { get; private set; }
        public Guid RatedEntityID { get; private set; }

        public RatingStars Stars { get; private set; }
        public string? Comment { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Rating() { }

        public Rating(
            OrderID orderId,
            UserID raterUserId,
            Guid ratedEntityId,
            RatingStars stars,
            RatedEntityType targetType,
            DateTimeOffset createdAtUtc,
            string? comment = null)
        {
            if (orderId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderId));

            if (raterUserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(raterUserId));

            if (ratedEntityId == Guid.Empty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ratedEntityId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (!Enum.IsDefined(typeof(RatedEntityType), targetType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(targetType));

            if (raterUserId.Value == ratedEntityId) throw new DomainRuleViolationException
                    (RatingErrors.SelfRatingCode, RatingErrors.SelfRatingMessage);

            ID = RatingID.New();
            OrderID = orderId;
            RaterUserID = raterUserId;

            TargetType = targetType;
            RatedEntityID = ratedEntityId;

            CreatedAt = createdAtUtc;

            SetStars(stars);
            SetComment(comment);
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Update(RatingStars newStars, string? newComment, DateTimeOffset utcNow)
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            if (utcNow < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(utcNow));

            if (utcNow > CreatedAt.AddDays(1)) throw new DomainRuleViolationException
                    (RatingErrors.EditExpiredCode, RatingErrors.EditExpiredMessage);

            SetStars(newStars);
            SetComment(newComment);
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetStars(RatingStars value)
        {
            if (!Enum.IsDefined(typeof(RatingStars), value)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(Stars));

            Stars = value;
        }

        private void SetComment(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Comment));

            Comment = value;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        private static string? NormalizeOptional(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}