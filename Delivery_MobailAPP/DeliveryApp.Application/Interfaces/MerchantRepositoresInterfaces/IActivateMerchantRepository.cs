using DeliveryApp.Domain.Entities.Merchants;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface IActivateMerchantRepository
    {
        Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default);

        Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default);

        Task<int> GetWorkingHoursCountAsync(MerchantID merchantId, CancellationToken ct = default);

        Task<int> GetActiveProductsCountAsync(MerchantID merchantId, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}