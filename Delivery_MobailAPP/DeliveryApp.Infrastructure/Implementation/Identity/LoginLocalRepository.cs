using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;

namespace DeliveryApp.Infrastructure.Implementation.Identity
{
    public sealed class LoginLocalRepository : ILoginLocalRepository
    {
        private readonly ApplicationDbContext _context;

        public LoginLocalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByPhoneAsync(string phone, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Phone == phone, ct);
        }

        public async Task<UserIdentity?> GetLocalIdentityAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.UserIdentities
                .FirstOrDefaultAsync(x => x.UserID == userId && x.Provider == AuthProvider.Local, ct);
        }

        public async Task<UserSession?> GetSessionAsync(UserID userId, ClientType clientType, CancellationToken ct = default)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(x => x.UserID == userId && x.ClientType == clientType, ct);
        }

        public async Task AddSessionAsync(UserSession session, CancellationToken ct = default)
        {
            await _context.UserSessions.AddAsync(session, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}