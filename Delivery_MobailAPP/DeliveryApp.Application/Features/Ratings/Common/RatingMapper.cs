using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Entities.Merchants;

namespace DeliveryApp.Application.Features.Ratings.Common
{
    public static class RatingMapper // يحول Entity الداخلي إلى DTO مفهوم للفرونت
    {
        public static RatingDto ToDto(Rating rating, Merchant merchant)
        {
            return new RatingDto
            {
                Id = rating.ID.Value,
                OrderId = rating.OrderID.Value,
                CustomerId = rating.RaterUserID.Value,
                MerchantId = rating.RatedEntityID,
                Stars = (int)rating.Stars,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt,
                MerchantAverageRating = merchant.AverageRating,
                MerchantRatingsCount = merchant.RatingsCount
            };
        }
    }
}