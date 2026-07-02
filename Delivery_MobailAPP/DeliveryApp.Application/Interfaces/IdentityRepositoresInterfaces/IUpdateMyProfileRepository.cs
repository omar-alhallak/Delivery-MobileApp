using DeliveryApp.Domain.Entities.Identity;

namespace DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces
{
    public interface IUpdateMyProfileRepository
    {
        Task<User?> GetUserByIdAsync(UserID userId, CancellationToken ct = default);

        Task<UserIdentity?> GetLocalIdentityAsync(UserID userId, CancellationToken ct = default);

        Task<bool> PhoneExistsAsync(string phone, UserID currentUserId, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}