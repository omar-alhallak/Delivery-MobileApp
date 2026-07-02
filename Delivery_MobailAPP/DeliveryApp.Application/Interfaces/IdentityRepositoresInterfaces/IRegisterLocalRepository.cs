using DeliveryApp.Domain.Entities.Identity;

namespace DeliveryApp.Application.Interfaces
{
    public interface IRegisterLocalRepository
    {
        Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default);

        Task AddUserAsync(User user, CancellationToken ct = default);

        Task AddUserIdentityAsync(UserIdentity identity, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}