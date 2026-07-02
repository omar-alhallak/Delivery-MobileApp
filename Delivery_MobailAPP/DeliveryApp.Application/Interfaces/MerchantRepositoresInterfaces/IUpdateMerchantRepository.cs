using DeliveryApp.Domain.Entities.Merchants;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface IUpdateMerchantRepository
    {
        Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default);

        Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default);

        Task<bool> SlugExistsAsync(string slug, MerchantID merchantId, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}