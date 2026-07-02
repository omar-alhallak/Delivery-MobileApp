namespace DeliveryApp.Application.Features.Orders.Common
{
    public sealed class OrderItemDto // DTO يرجع تفاصيل منتج واحد داخل الطلب
    {
        public Guid Id { get; init; } // معرف المنتج داخل الطلب
        public string ProductName { get; init; } = null!; // اسم المنتج المحفوظ وقت الطلب
        public string? VariantName { get; init; } // النوع أو الحجم
        public decimal UnitPrice { get; init; } // سعر الوحدة
        public int Quantity { get; init; } // الكمية
        public decimal LineTotal { get; init; } // سعر السطر كامل
        public string? CustomerNote { get; init; } // ملاحظة الزبون
    }
}