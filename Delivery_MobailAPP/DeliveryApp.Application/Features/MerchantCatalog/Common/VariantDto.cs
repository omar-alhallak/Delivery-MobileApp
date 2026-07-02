namespace DeliveryApp.Application.Features.MerchantCatalog.Common
{
    public sealed class VariantDto // DTO يرجع خيار المنتج للواجهة
    {
        public Guid Id { get; init; } // معرف الخيار
        public Guid ProductId { get; init; } // المنتج المرتبط
        public string VariantName { get; init; } = null!; // اسم الخيار مثل صغير أو كبير
        public decimal BasePrice { get; init; } // السعر
        public int SortOrder { get; init; } // ترتيب العرض
        public bool IsActive { get; init; } // هل الخيار مفعل
        public DateTimeOffset CreatedAt { get; init; } // وقت الإنشاء
    }
}
