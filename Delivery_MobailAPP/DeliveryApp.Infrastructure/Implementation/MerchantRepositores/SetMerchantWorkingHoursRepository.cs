using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.Merchants.Repositores
{
    public sealed class SetMerchantWorkingHoursRepository : ISetMerchantWorkingHoursRepository
    {
        private readonly ApplicationDbContext _context;

        public SetMerchantWorkingHoursRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await _context.Merchants.FirstOrDefaultAsync(x => x.ID == merchantId, ct);
        }

        public async Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default)
        {
            return await _context.MerchantUsers
                .FirstOrDefaultAsync(x => x.MerchantID == merchantId && x.UserID == userId, ct);
        }

        public async Task<IReadOnlyList<MerchantWorkingHour>> GetWorkingHoursAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await _context.MerchantWorkingHours.Where(x => x.MerchantID == merchantId).ToListAsync(ct);
        }

        public async Task AddWorkingHourAsync(MerchantWorkingHour workingHour, CancellationToken ct = default)
        {
            await _context.MerchantWorkingHours.AddAsync(workingHour, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}