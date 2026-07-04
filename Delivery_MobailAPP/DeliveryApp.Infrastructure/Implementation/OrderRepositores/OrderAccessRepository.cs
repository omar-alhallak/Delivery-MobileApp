using Microsoft.EntityFrameworkCore;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.OrderRepositores
{
    public sealed class OrderAccessRepository : IOrderAccessRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderAccessRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<bool> HasActiveMerchantAccessAsync(UserID userId, MerchantID merchantId, CancellationToken ct = default)
        {
            return (
                from access in _context.MerchantUsers
                join merchant in _context.Merchants on access.MerchantID equals merchant.ID
                where access.UserID == userId
                    && access.MerchantID == merchantId
                    && access.IsActive
                    && merchant.IsActive
                select access)
                .AnyAsync(ct);
        }
    }
}