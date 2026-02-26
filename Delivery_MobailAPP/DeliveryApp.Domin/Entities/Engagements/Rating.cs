using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DeliveryApp.Domain.Entities.Customers.Order;
using DeliveryApp.Domain.Entities.DriverRequest;
using DeliveryApp.Domain.Entities.Identity;

namespace DeliveryApp.Domain.Entities.Feedback
{
    public class Rating
    {
        public Guid RatingId { get; private set; }

        public Guid OrderId { get; private set; }
        public Order? Order { get; private set; }

        public Guid RaterUserId { get; private set; }
       
        public User? RaterUser { get; private set; }
        public RatedEntityType? RaterEntityType { get; private set; }

        public Guid RatedEntityId { get; private set; }
        public  User? RaterEntity { get; private set; }

        public int Stars { get; private set; }

        public string? Comment { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Rating() { }

        public void Create(Guid orderId, Guid raterId, Guid ratedId, RatedEntityType type, int stars, string? comment)
        {
            RatingId = Guid.NewGuid();
            OrderId = orderId;
            RaterUserId = raterId;
            RatedEntityId = ratedId;
            Stars = stars;
            Comment = comment;
            CreatedAt = DateTimeOffset.UtcNow;
            var normalizedComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        }

        public void Update(int newStars, string? newComment,Guid orderId, Guid raterId, Guid ratedId, RatedEntityType type, int stars, string? comment)
        {


            if (stars < 1 || stars > 5)
                throw new ArgumentException("The rating should be between 1 and 5 stars.");

            if (orderId == Guid.Empty || raterId == Guid.Empty || ratedId == Guid.Empty)
                throw new ArgumentException("IDs cannot be empty.");

            if (orderId == Guid.Empty) throw new ArgumentException("The assessment must be linked to a valid request.");

            if (raterId == ratedId) throw new ArgumentException("It is illogical for a user to evaluate himself.");

            if (comment != null && comment?.Length > RatingConstraints.MaxCommentLength)
                throw new ArgumentException($"The comment was over {RatingConstraints.MaxCommentLength} character.");
            if (DateTimeOffset.UtcNow > CreatedAt.AddHours(24))
                throw new InvalidOperationException("Ratings cannot be edited after 24 hours from the time they are created.");

            if (newStars < 1 || newStars > 5)
                throw new ArgumentOutOfRangeException(nameof(newStars));

            Stars = newStars;
            Comment = newComment;
        }

        public static class RatingConstraints
        {
            public const int MaxCommentLength = 1000;
            public const int MinStars = 1;
            public const int MaxStars = 5;
        }
        

    }
}