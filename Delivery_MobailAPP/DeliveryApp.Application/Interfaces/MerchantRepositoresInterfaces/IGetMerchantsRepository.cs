using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Application.Features.Merchants.GetMerchants;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface IGetMerchantsRepository
    {
        Task<IReadOnlyList<GetMerchantsResponse>> GetActiveMerchantsAsync(MerchantType merchantType, CancellationToken ct = default);

        Task<IReadOnlyList<GetMerchantsResponse>> GetActiveMerchantsBySystemCategoryAsync(MerchantType merchantType, Guid systemCategoryId, CancellationToken ct = default);
    }
}