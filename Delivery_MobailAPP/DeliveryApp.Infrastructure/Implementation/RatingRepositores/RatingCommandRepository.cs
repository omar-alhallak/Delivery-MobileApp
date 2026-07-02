using DeliveryApp.Application.Interfaces.RatingRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OrderID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.RatingRepositores
{
    public sealed class RatingCommandRepository : IRatingCommandRepository // تنفيذ أوامر التقييمات من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context;

        public RatingCommandRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order?> GetOrderAsync(OrderID orderId, CancellationToken ct = default)
            => await _context.Orders.FirstOrDefaultAsync(x => x.ID == orderId, ct);

        public async Task<Merchant?> GetMerchantAsync(MerchantID merchantId, CancellationToken ct = default)
            => await _context.Merchants.FirstOrDefaultAsync(x => x.ID == merchantId, ct);

        public async Task<Rating?> GetRatingByOrderAsync(OrderID orderId, CancellationToken ct = default)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(x => x.OrderID == orderId && x.TargetType == RatedEntityType.Merchant, ct);
        }

        public async Task AddRatingAsync(Rating rating, CancellationToken ct = default)
            => await _context.Ratings.AddAsync(rating, ct);

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}
