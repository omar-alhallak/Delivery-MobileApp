using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Domain.DomainErrors.EngagementErrors;

namespace DeliveryApp.Domain.Entities.Engagements
{
    public class Rating // يمثل تقييم حهة معينة
    {
        // -------------------------
        //            Key
        // -------------------------

        public RatingID ID { get; private set; } // PK معرف التقييم

        // -------------------------
        //         Relations
        // -------------------------

        public OrderID OrderID { get; private set; } // الطلب المرتبط بالتقييم
        public UserID RaterUserID { get; private set; } // المستخدم الي قام بالتقييم

        public RatedEntityType TargetType { get; private set; } // الكيان الي تقيم
        public Guid RatedEntityId { get; private set; } // معرف الكيان الي تقيم

        // -------------------------
        //         Content
        // -------------------------

        public RatingStars Stars { get; private set; } // عدد النجوم
        public string? Comment { get; private set; } // تعليق اختياري

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء التقييم

        private Rating() { }

        public Rating(OrderID orderId, UserID raterUserId, Guid ratedEntityId, RatingStars stars, RatedEntityType targetType, DateTimeOffset createdAtUtc, string? comment = null)
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

            if ((targetType == RatedEntityType.Customer || targetType == RatedEntityType.Driver) && raterUserId.Value == ratedEntityId)
                throw new DomainRuleViolationException (RatingErrors.SelfRatingCode, RatingErrors.SelfRatingMessage);

            ID = RatingID.New();
            OrderID = orderId;
            RaterUserID = raterUserId;

            TargetType = targetType;
            RatedEntityId = ratedEntityId;

            CreatedAt = createdAtUtc;

            SetStars(stars);
            SetComment(comment);
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Update(RatingStars newStars, string? newComment, DateTimeOffset utcNow) // تعديل التقييم
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

        private void SetStars(RatingStars value) // إدخال عدد النجوم
        {
            if (!Enum.IsDefined(typeof(RatingStars), value)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(Stars));

            Stars = value;
        }

        private void SetComment(string? value) // إدخال التعليق
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Comment));

            Comment = value;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim(); // تنظيف النص
    }
}