using Microsoft.EntityFrameworkCore;
using DeliveryApp.Application.Interfaces;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Infrastructure.Persistence;

namespace DeliveryApp.Infrastructure.Implementation.Identity
{
    public sealed class RegisterLocalRepository : IRegisterLocalRepository
    {
        private readonly ApplicationDbContext _context;

        public RegisterLocalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.Phone == phone, ct);
        }

        public async Task AddUserAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task AddUserIdentityAsync(UserIdentity identity, CancellationToken ct = default)
        {
            await _context.UserIdentities.AddAsync(identity, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}