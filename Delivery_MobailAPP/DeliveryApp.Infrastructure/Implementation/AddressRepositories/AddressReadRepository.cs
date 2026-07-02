using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Customers;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.AddressRepositories
{
    public sealed class AddressReadRepository : IAddressReadRepository // تنفيذ قراءة العناوين من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context;

        public AddressReadRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<Address>> GetByUserAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.Addresses
                .AsNoTracking()
                .Where(x => x.UserID == userId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.IsActive)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<Address?> GetByIdAsync(AddressID addressId, CancellationToken ct = default)
        {
            return await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == addressId, ct);
        }
    }
}
