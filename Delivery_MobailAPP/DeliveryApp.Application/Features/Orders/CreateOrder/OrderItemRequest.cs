namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public sealed class OrderItemRequest // DTO يمثل منتج واحد داخل طلب الإنشاء
    {
        public Guid ProductId { get; init; } // المنتج المختار من كتالوج المطعم
        public Guid? VariantId { get; init; } // الحجم أو الخيار المختار إذا موجود
        public int Quantity { get; init; } // الكمية المطلوبة
        public string? CustomerNote { get; init; } // ملاحظة الزبون على هذا المنتج
    }
}
