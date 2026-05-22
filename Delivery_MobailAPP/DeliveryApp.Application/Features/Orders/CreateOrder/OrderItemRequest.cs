namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public sealed class OrderItemRequest // DTO يمثل منتج واحد داخل طلب الإنشاء
    {
        public string ProductName { get; init; } = null!; // اسم المنتج كما يظهر وقت الطلب
        public string? VariantName { get; init; } // اسم النوع أو الحجم إذا موجود
        public decimal UnitPrice { get; init; } // سعر القطعة الواحدة
        public int Quantity { get; init; } // الكمية المطلوبة
        public string? CustomerNote { get; init; } // ملاحظة الزبون على هذا المنتج
    }
}
