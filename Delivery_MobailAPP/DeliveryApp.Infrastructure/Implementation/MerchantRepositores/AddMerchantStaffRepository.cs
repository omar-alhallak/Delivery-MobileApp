using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.Merchants.Repositores
{
    public sealed class AddMerchantStaffRepository : IAddMerchantStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public AddMerchantStaffRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Merchant?> GetMerchantByIdAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await _context.Merchants.FirstOrDefaultAsync(x => x.ID == merchantId, ct);
        }

        public async Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default)
        {
            return await _context.MerchantUsers.FirstOrDefaultAsync(x => x.MerchantID == merchantId && x.UserID == userId, ct);
        }

        public async Task<User?> GetUserByPublicCodeAsync(PublicCode publicCode, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.PublicID == publicCode, ct);
        }

        public async Task AddMerchantUserAsync(MerchantUser merchantUser, CancellationToken ct = default)
        {
            await _context.MerchantUsers.AddAsync(merchantUser, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}