using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.Merchants.CreateMerchant
{
    public sealed record CreateMerchantRequest
    {
        public MerchantType MerchantType { get; init; }

        public string MerchantName { get; init; } = null!;
        public string Slug { get; init; } = null!;

        public string? Description { get; init; }
        public string? Phone { get; init; }

        public string? LogoUrl { get; init; }
        public string? CoverImageUrl { get; init; }

        public double Latitude { get; init; }
        public double Longitude { get; init; }

        public int DefaultPreparationMinutes { get; init; }
    }
}