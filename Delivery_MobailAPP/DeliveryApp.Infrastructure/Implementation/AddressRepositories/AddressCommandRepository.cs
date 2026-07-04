using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Customers;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.AddressRepositories
{
    public sealed class AddressCommandRepository : IAddressCommandRepository // تنفيذ أوامر العناوين من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context;

        public AddressCommandRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> UserExistsAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.ID == userId, ct);
        }

        public async Task<Address?> GetByIdAsync(AddressID addressId, UserID userId, CancellationToken ct = default)
        {
            return await _context.Addresses.FirstOrDefaultAsync(x => x.ID == addressId && x.UserID == userId, ct);
        }

        public async Task<IReadOnlyList<Address>> GetUserAddressesAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.Addresses
                .Where(x => x.UserID == userId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Address address, CancellationToken ct = default)
        {
            await _context.Addresses.AddAsync(address, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
