using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces
{
    public interface ILogoutRepository
    {
        Task<UserSession?> GetSessionAsync(UserID userId, ClientType clientType, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}