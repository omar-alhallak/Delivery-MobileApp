using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Identity;

namespace DeliveryApp.Application.Interfaces
{
    public interface IIdentityDbContext
    {
        DbSet<User> Users { get; }
        DbSet<UserIdentity> UserIdentities { get; }
        DbSet<UserSession> UserSessions { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}