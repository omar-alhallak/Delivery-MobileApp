using Microsoft.EntityFrameworkCore;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Application.Interfaces.RatingRepositoriesInterfaces;
using OrderID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.RatingRepositores
{
    public sealed class RatingReadRepository : IRatingReadRepository // تنفيذ قراءة التقييمات من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context;

        public RatingReadRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Rating?> GetByOrderAsync(OrderID orderId, CancellationToken ct = default) // جلب تقييم طلب محدد
        {
            return await _context.Ratings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderID == orderId && x.TargetType == RatedEntityType.Merchant, ct);
        }

        public async Task<IReadOnlyList<Rating>> GetByMerchantAsync(MerchantID merchantId, CancellationToken ct = default) // جلب تقييمات مطعم
        {
            return await _context.Ratings
                .AsNoTracking()
                .Where(x => x.TargetType == RatedEntityType.Merchant && x.RatedEntityID == merchantId.Value)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
        }
    }
}