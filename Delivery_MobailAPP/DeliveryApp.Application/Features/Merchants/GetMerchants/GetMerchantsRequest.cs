using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.Merchants.GetMerchants
{
    public sealed record GetMerchantsRequest
    {
        public MerchantType MerchantType { get; init; }

        public Guid? SystemCategoryId { get; init; }
    }
}