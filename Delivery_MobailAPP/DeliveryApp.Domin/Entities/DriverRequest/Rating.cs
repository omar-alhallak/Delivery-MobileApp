using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.DriverRequest
{
    public enum RatedEntityType : byte
    {
        Merchant = 1,
        Driver = 2,
        Product = 3
    }

    public class Rating
    {
        public Guid RatingID { get; private set; }
        public Guid OrderID { get; private set; }
        public Guid RaterUserID { get; private set; } // الشخص الذي قام بالتقييم

        public Guid RatedEntityID { get; private set; } // المعرف الخاص بالمقيم (سائق/متجر)
        public RatedEntityType RatedEntityType { get; private set; }

        public byte Stars { get; private set; } // من 1 إلى 5
        public string? Comment { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Rating() { } // للـ EF Core

        public Rating(
            Guid orderId,
            Guid raterUserId,
            Guid ratedEntityId,
            RatedEntityType entityType,
            byte stars,
            string? comment)
        {
            if (stars < 1 || stars > 5)
                throw new ArgumentException("Rating must be between 1 and 5 stars.");

            OrderID = orderId;
            RaterUserID = raterUserId;
            RatedEntityID = ratedEntityId;
            RatedEntityType = entityType;
            Stars = stars;
            Comment = NormalizeComment(comment);
            CreatedAt = DateTimeOffset.UtcNow;
        }

        private string? NormalizeComment(string? comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return null;
            comment = comment.Trim();
            return comment.Length > 1000 ? comment.Substring(0, 1000) : comment;
        }
    }
}
