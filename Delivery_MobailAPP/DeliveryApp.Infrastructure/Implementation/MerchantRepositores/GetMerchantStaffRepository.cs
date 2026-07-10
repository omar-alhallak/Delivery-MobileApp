using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Repositories.MerchantRepositories
{
    public sealed class GetMerchantStaffRepository
        : IGetMerchantStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public GetMerchantStaffRepository(ApplicationDbContext context)
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

        public async Task<IReadOnlyList<MerchantStaffData>> GetMerchantStaffAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await
            (
                from merchantUser in _context.MerchantUsers.AsNoTracking()

                join user in _context.Users.AsNoTracking()
                    on merchantUser.UserID equals user.ID

                where merchantUser.MerchantID == merchantId
                      && merchantUser.Role == MerchantUserRole.Staff

                orderby merchantUser.IsActive descending,
                        merchantUser.CreatedAt descending

                select new MerchantStaffData
                (
                    merchantUser.UserID,
                    user.FullName,
                    user.Phone,
                    user.PhotoUrl,
                    merchantUser.Role,
                    merchantUser.IsActive,
                    merchantUser.CreatedAt
                )
            ).ToListAsync(ct);
        }
    }
}