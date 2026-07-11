using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.Merchants.GetMerchantDetails
{
    public sealed class GetMerchantDetailsResponse
    {
        public Guid MerchantId { get; set; }

        public string? PublicId { get; set; }

        public MerchantType MerchantType { get; set; }

        public string MerchantName { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Phone { get; set; }

        public string? LogoUrl { get; set; }

        public string? CoverImageUrl { get; set; }

        public int DefaultPreparationMinutes { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public decimal AverageRating { get; set; }

        public int RatingsCount { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}