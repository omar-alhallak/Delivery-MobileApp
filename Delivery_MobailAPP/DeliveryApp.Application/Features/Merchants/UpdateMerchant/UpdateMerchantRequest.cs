namespace DeliveryApp.Application.Features.Merchants.UpdateMerchant
{
    public sealed record UpdateMerchantRequest
    {
        public Guid MerchantId { get; init; }

        public string? MerchantName { get; init; }
        public string? Slug { get; init; }

        public string? Description { get; init; }
        public string? Phone { get; init; }

        public string? LogoUrl { get; init; }
        public string? CoverImageUrl { get; init; }

        public double? Latitude { get; init; }
        public double? Longitude { get; init; }

        public int? DefaultPreparationMinutes { get; init; }
    }
}