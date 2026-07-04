using Microsoft.EntityFrameworkCore;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.MerchantCatalogRepositories
{
    public sealed class MerchantCatalogAccessRepository : IMerchantCatalogAccessRepository // تنفيذ فحص صلاحيات الكتالوج من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context;

        public MerchantCatalogAccessRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<bool> HasActiveAccessAsync(UserID userId, MerchantID merchantId, CancellationToken ct = default)
        {
            return _context.MerchantUsers.AnyAsync(
                x => x.UserID == userId && x.MerchantID == merchantId && x.IsActive,
                ct);
        }

        public async Task<IReadOnlyList<MyMerchantDto>> GetUserMerchantsAsync(UserID userId, CancellationToken ct = default)
        {
            var rows = await (
                from access in _context.MerchantUsers.AsNoTracking()
                join merchant in _context.Merchants.AsNoTracking()
                    on access.MerchantID equals merchant.ID
                where access.UserID == userId && access.IsActive
                orderby merchant.CreatedAt
                select new { Access = access, Merchant = merchant })
                .ToListAsync(ct);

            return rows.Select(x => new MyMerchantDto
            {
                MerchantId = x.Merchant.ID.Value,
                MerchantName = x.Merchant.MerchantName.Value,
                Role = x.Access.Role,
                IsActive = x.Merchant.IsActive
            }).ToList();
        }
    }
}