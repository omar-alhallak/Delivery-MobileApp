using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Customers;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;
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

        public async Task SwitchDefaultAsync(Address address, CancellationToken ct = default)
        {
            if (address.IsDefault) return;

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            var currentDefaultAddresses = await _context.Addresses
                .Where(x => x.UserID == address.UserID && x.ID != address.ID && x.IsDefault)
                .ToListAsync(ct);

            foreach (var currentDefaultAddress in currentDefaultAddresses)
            {
                currentDefaultAddress.RemoveDefault();
            }

            // نحفظ إلغاء الافتراضي القديم أولاً حتى لا يتعارض مع الـ unique index.
            await _context.SaveChangesAsync(ct);

            address.SetAsDefault();
            await _context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
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
