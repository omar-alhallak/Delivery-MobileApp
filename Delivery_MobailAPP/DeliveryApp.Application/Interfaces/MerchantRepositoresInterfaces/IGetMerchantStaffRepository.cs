using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface IGetMerchantStaffRepository
    {
        Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default);

        Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default);

        Task<IReadOnlyList<MerchantStaffData>> GetMerchantStaffAsync(MerchantID merchantId, CancellationToken ct = default);
    }

    public sealed record MerchantStaffData
    (
        UserID UserID,
        string? FullName,
        string? Phone,
        string? PhotoUrl,
        MerchantUserRole Role,
        bool IsActive,
        DateTimeOffset CreatedAt
    );
}