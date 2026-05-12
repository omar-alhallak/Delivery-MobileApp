using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Application.Interfaces.IdentityInterfaces
{
    public interface ILoginLocalRepository
    {
        Task<User?> GetUserByPhoneAsync(string phone, CancellationToken ct = default);

        Task<UserIdentity?> GetLocalIdentityAsync(UserID userId, CancellationToken ct = default);

        Task<UserSession?> GetSessionAsync(UserID userId, ClientType clientType, CancellationToken ct = default);

        Task AddSessionAsync(UserSession session, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}