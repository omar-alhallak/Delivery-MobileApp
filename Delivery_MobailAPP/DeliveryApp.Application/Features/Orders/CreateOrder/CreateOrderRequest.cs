using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public sealed class CreateOrderRequest // DTO يستقبل بيانات إنشاء الطلب من الواجهة
    {
        public OrderType OrderType { get; init; } // نوع الطلب
        public Guid MerchantId { get; init; } // المطعم الذي سيستقبل الطلب
        public Guid AddressId { get; init; } // عنوان التسليم التابع للزبون الحالي
        public decimal TipAmount { get; init; } // الإكرامية إن وجدت
        public IReadOnlyList<OrderItemRequest> Items { get; init; } = []; // المنتجات المطلوبة
    }
}
