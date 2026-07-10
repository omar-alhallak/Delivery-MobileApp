using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Repositories.MerchantRepositories
{
    public sealed class GetMerchantWorkingHoursRepository : IGetMerchantWorkingHoursRepository
    {
        private readonly ApplicationDbContext _context;

        public GetMerchantWorkingHoursRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return _context.Merchants.AsNoTracking().FirstOrDefaultAsync(x => x.ID == merchantId, ct);
        }

        public Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default)
        {
            return _context.MerchantUsers.AsNoTracking().FirstOrDefaultAsync(
                    x => x.MerchantID == merchantId && x.UserID == userId, ct);
        }

        public async Task<IReadOnlyList<MerchantWorkingHour>> GetWorkingHoursAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await _context.MerchantWorkingHours
                .AsNoTracking()
                .Where(x => x.MerchantID == merchantId)
                .ToListAsync(ct);
        }
    }
}