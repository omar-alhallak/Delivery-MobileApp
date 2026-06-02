using DeliveryApp.Domain.Entities.Merchants;

namespace DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces
{
    public interface ICreateMerchantRepository
    {
        Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);

        Task AddMerchantAsync(Merchant merchant, CancellationToken ct = default);

        Task AddMerchantUserAsync(MerchantUser merchantUser, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}