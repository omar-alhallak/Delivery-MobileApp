using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.IdentityRepositores
{
    public sealed class GetMyProfileRepository : IGetMyProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public GetMyProfileRepository(ApplicationDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetUserByIdAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.ID == userId, ct);
        }
    }
}