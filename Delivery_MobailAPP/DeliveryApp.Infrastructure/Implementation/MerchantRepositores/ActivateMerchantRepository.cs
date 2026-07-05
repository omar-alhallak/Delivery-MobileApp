using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.Merchants.Repositores
{
    public sealed class ActivateMerchantRepository : IActivateMerchantRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivateMerchantRepository(ApplicationDbContext context)
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

        public async Task<int> GetWorkingHoursCountAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await _context.MerchantWorkingHours.CountAsync(x => x.MerchantID == merchantId, ct);
        }

        public async Task<int> GetActiveProductsCountAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await (from category in _context.MerchantCategories
                          join product in _context.Products
                              on category.ID equals product.MerchantCategoryID
                          where category.MerchantID == merchantId && category.IsActive && product.IsActive
                          select product.ID).CountAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}