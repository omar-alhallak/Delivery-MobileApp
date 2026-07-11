using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Application.Features.Merchants.GetMerchantDetails;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface IGetMerchantDetailsRepository
    {
        Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default);

        Task<GetMerchantDetailsResponse?> GetMerchantDetailsAsync(MerchantID merchantId, CancellationToken ct = default);
    }
}