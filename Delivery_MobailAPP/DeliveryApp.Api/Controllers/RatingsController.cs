using Microsoft.AspNetCore.Mvc;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Application.Features.Ratings;
using DeliveryApp.Application.Features.Ratings.Common;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/ratings")]
    public sealed class RatingsController : ControllerBase // Controller خاص بتقييم المطعم بعد تسليم الطلب
    {
        private readonly RatingService _ratingService;

        public RatingsController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost("orders/{orderId:guid}")]
        public Task<IActionResult> Create(Guid orderId, [FromBody] RatingRequest request, CancellationToken ct) // إنشاء تقييم للطلب
            => RunCreate(() => _ratingService.CreateAsync(orderId, request, ct));

        [HttpPut("orders/{orderId:guid}")]
        public Task<IActionResult> Update(Guid orderId, [FromBody] RatingRequest request, CancellationToken ct) // تعديل تقييم الطلب
            => RunUpdate(() => _ratingService.UpdateAsync(orderId, request, ct));

        [HttpGet("orders/{orderId:guid}")]
        public Task<IActionResult> GetOrderRating(Guid orderId, CancellationToken ct) // جلب تقييم طلب محدد
            => RunUpdate(() => _ratingService.GetOrderRatingAsync(orderId, ct));

        [HttpGet("merchants/{merchantId:guid}")]
        public async Task<ActionResult<IReadOnlyList<RatingDto>>> GetMerchantRatings(Guid merchantId, CancellationToken ct) // جلب تقييمات مطعم
            => Ok(await _ratingService.GetMerchantRatingsAsync(merchantId, ct));

        private static async Task<IActionResult> RunCreate<T>(Func<Task<T>> action) // توحيد ردود الإنشاء
        {
            try
            {
                return new OkObjectResult(await action());
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }

        private static async Task<IActionResult> RunUpdate<T>(Func<Task<T?>> action) where T : class // توحيد ردود التعديل والجلب
        {
            try
            {
                var response = await action();
                return response is null ? new NotFoundResult() : new OkObjectResult(response);
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }
    }
}