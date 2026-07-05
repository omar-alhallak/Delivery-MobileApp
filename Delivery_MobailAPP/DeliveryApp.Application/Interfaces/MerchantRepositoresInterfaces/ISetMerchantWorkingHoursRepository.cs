using DeliveryApp.Domain.Entities.Merchants;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface ISetMerchantWorkingHoursRepository
    {
        Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default);

        Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default);

        Task<IReadOnlyList<MerchantWorkingHour>> GetWorkingHoursAsync(MerchantID merchantId, CancellationToken ct = default);

        Task AddWorkingHourAsync(MerchantWorkingHour workingHour, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}