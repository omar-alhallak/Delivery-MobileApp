using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface IAddMerchantStaffRepository
    {
        Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default);

        Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default);

        Task<User?> GetUserByPublicCodeAsync(PublicCode publicCode, CancellationToken ct = default);

        Task AddMerchantUserAsync(MerchantUser merchantUser, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}