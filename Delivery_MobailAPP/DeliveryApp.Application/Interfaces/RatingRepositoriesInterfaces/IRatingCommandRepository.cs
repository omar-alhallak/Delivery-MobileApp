using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Entities.Engagements;
using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Interfaces.RatingRepositoriesInterfaces
{
    public interface IRatingCommandRepository // عقد تنفيذ أوامر التقييمات والحفظ
    {
        Task<Order?> GetOrderAsync(OrderID orderId, CancellationToken ct = default); // جلب الطلب لمعرفة الزبون والمطعم والحالة
        Task<Merchant?> GetMerchantAsync(MerchantID merchantId, CancellationToken ct = default); // جلب المطعم لتحديث متوسط التقييم
        Task<Rating?> GetRatingByOrderAsync(OrderID orderId, CancellationToken ct = default); // جلب تقييم الطلب إن وجد
        Task AddRatingAsync(Rating rating, CancellationToken ct = default); // إضافة تقييم جديد
        Task SaveChangesAsync(CancellationToken ct = default); // حفظ التغييرات
    }
}