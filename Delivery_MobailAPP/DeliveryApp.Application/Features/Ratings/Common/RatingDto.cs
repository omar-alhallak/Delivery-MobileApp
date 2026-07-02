namespace DeliveryApp.Application.Features.Ratings.Common
{
    public sealed class RatingDto // البيانات التي ترجع للفرونت بعد إنشاء أو تعديل أو جلب التقييم
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public Guid MerchantId { get; init; }
        public int Stars { get; init; }
        public string? Comment { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public decimal MerchantAverageRating { get; init; }
        public int MerchantRatingsCount { get; init; }
    }
}
