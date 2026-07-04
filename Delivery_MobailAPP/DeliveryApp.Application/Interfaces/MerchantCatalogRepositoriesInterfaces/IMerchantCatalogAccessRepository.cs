using DeliveryApp.Application.Features.MerchantCatalog.Access;

namespace DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces
{
    public interface IMerchantCatalogAccessRepository // عقد التحقق من ارتباط المستخدم بالمطعم
    {
        Task<bool> HasActiveAccessAsync(UserID userId, MerchantID merchantId, CancellationToken ct = default);

        Task<IReadOnlyList<MyMerchantDto>> GetUserMerchantsAsync(UserID userId, CancellationToken ct = default);
    }
}
