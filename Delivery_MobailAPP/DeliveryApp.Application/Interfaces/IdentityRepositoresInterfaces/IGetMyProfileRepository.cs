using DeliveryApp.Domain.Entities.Identity;

namespace DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces
{
    public interface IGetMyProfileRepository
    {
        Task<User?> GetUserByIdAsync(UserID userId, CancellationToken ct = default);
    }
}