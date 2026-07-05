namespace DeliveryApp.Application.Features.Merchants.GetMerchants
{
    public sealed record GetMerchantsResponse
    {
        public Guid MerchantId { get; init; }
        public string PublicId { get; init; } = null!;

        public string MerchantName { get; init; } = null!;
        public string Slug { get; init; } = null!;

        public string? LogoUrl { get; init; }
        public string? CoverImageUrl { get; init; }

        public decimal AverageRating { get; init; }
        public int RatingsCount { get; init; }

        public int DefaultPreparationMinutes { get; init; }
    }
}