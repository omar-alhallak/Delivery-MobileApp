using DeliveryApp.Domain.Entities.Engagements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Application.Interfaces.RatingRepositoriesInterfaces
{
    public interface IRatingReadRepository // عقد قراءة التقييمات بدون معرفة تفاصيل قاعدة البيانات
    {
        Task<Rating?> GetByOrderAsync(OrderID orderId, CancellationToken ct = default); // جلب تقييم طلب محدد
        Task<IReadOnlyList<Rating>> GetByMerchantAsync(MerchantID merchantId, CancellationToken ct = default); // جلب تقييمات مطعم
    }
}
