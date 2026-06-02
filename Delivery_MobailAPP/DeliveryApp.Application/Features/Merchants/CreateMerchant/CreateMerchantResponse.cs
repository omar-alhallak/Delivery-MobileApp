using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.Merchants.CreateMerchant
{
    public sealed record CreateMerchantResponse
    {
        public Guid MerchantId { get; init; }
        public string PublicId { get; init; } = null!;

        public string MerchantName { get; init; } = null!;
        public string Slug { get; init; } = null!;

        public MerchantType MerchantType { get; init; }

        public bool IsActive { get; init; }
    }
}