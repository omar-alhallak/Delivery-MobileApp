using DeliveryApp.Application.Features.Ratings.Common;
using DeliveryApp.Application.Interfaces.RatingRepositoriesInterfaces;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Domain.Enums.OrderEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Application.Features.Ratings
{
    public sealed class RatingService // Use case خاص بتقييم المطعم بعد انتهاء الطلب
    {
        private readonly IRatingReadRepository _readRepository;
        private readonly IRatingCommandRepository _commandRepository;

        public RatingService(IRatingReadRepository readRepository, IRatingCommandRepository commandRepository)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
        }

        public async Task<RatingDto> CreateAsync(Guid orderId, RatingRequest request, CancellationToken ct = default) // إنشاء تقييم لأول مرة
        {
            if (request is null) throw new DomainValidationException("Rating.Request_Required", "Rating request is required.");

            var order = await _commandRepository.GetOrderAsync(OrderID.From(orderId), ct);
            if (order is null) throw new DomainRuleViolationException("Rating.Order_Not_Found", "Order was not found.");
            if (order.Status != OrderStatus.Delivered) throw new DomainRuleViolationException("Rating.Order_Not_Delivered", "Order must be delivered before rating.");
            if (!order.MerchantID.HasValue) throw new DomainRuleViolationException("Rating.Merchant_Required", "Order has no merchant to rate.");

            var existingRating = await _commandRepository.GetRatingByOrderAsync(order.ID, ct);
            if (existingRating is not null) throw new DomainRuleViolationException("Rating.Already_Exists", "Order already has a rating.");

            var merchant = await _commandRepository.GetMerchantAsync(order.MerchantID.Value, ct);
            if (merchant is null) throw new DomainRuleViolationException("Rating.Merchant_Not_Found", "Merchant was not found.");

            var stars = RatingValidator.ValidateStars(request.Stars);
            var rating = new Rating(order.ID, order.CustomerID, merchant.ID.Value, stars, RatedEntityType.Merchant, DateTimeOffset.UtcNow, request.Comment);

            merchant.AddRating(stars);

            await _commandRepository.AddRatingAsync(rating, ct);
            await _commandRepository.SaveChangesAsync(ct);

            return RatingMapper.ToDto(rating, merchant);
        }

        public async Task<RatingDto?> UpdateAsync(Guid orderId, RatingRequest request, CancellationToken ct = default) // تعديل تقييم موجود بأي وقت
        {
            if (request is null) throw new DomainValidationException("Rating.Request_Required", "Rating request is required.");

            var rating = await _commandRepository.GetRatingByOrderAsync(OrderID.From(orderId), ct);
            if (rating is null) return null;

            var merchant = await _commandRepository.GetMerchantAsync(MerchantID.From(rating.RatedEntityID), ct);
            if (merchant is null) throw new DomainRuleViolationException("Rating.Merchant_Not_Found", "Merchant was not found.");

            var oldStars = rating.Stars;
            var newStars = RatingValidator.ValidateStars(request.Stars);

            rating.Update(newStars, request.Comment, DateTimeOffset.UtcNow);
            merchant.UpdateRating(oldStars, newStars);

            await _commandRepository.SaveChangesAsync(ct);
            return RatingMapper.ToDto(rating, merchant);
        }

        public async Task<RatingDto?> GetOrderRatingAsync(Guid orderId, CancellationToken ct = default) // جلب تقييم طلب واحد
        {
            var rating = await _readRepository.GetByOrderAsync(OrderID.From(orderId), ct);
            if (rating is null) return null;

            var merchant = await _commandRepository.GetMerchantAsync(MerchantID.From(rating.RatedEntityID), ct);
            if (merchant is null) return null;

            return RatingMapper.ToDto(rating, merchant);
        }

        public async Task<IReadOnlyList<RatingDto>> GetMerchantRatingsAsync(Guid merchantId, CancellationToken ct = default) // جلب كل تقييمات مطعم
        {
            var merchant = await _commandRepository.GetMerchantAsync(MerchantID.From(merchantId), ct);
            if (merchant is null) return [];

            var ratings = await _readRepository.GetByMerchantAsync(merchant.ID, ct);
            return ratings.Select(rating => RatingMapper.ToDto(rating, merchant)).ToList();
        }
    }
}
